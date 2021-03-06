﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Bowerbird.Web.Infrastructure;
using Raven.Client;
using System.Collections;
using System.Dynamic;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelQuery _sightingViewModelQuery;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionManager _permissionManager;
        private readonly ISightingNoteViewModelQuery _sightingNoteViewModelQuery;
        private readonly IIdentificationViewModelQuery _identificationViewModelQuery;

        #endregion

        #region Constructors

        public ObservationsController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISightingViewModelQuery sightingViewModelQuery,
            IDocumentSession documentSession,
            IPermissionManager permissionManager,
            ISightingNoteViewModelQuery sightingNoteViewModelQuery,
            IIdentificationViewModelQuery identificationViewModelQuery
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelQuery, "sightingViewModelQuery");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(sightingNoteViewModelQuery, "sightingNoteViewModelQuery");
            Check.RequireNotNull(identificationViewModelQuery, "identificationViewModelQuery");

            _messageBus = messageBus;
            _userContext = userContext;
            _sightingViewModelQuery = sightingViewModelQuery;
            _documentSession = documentSession;
            _permissionManager = permissionManager;
            _sightingNoteViewModelQuery = sightingNoteViewModelQuery;
            _identificationViewModelQuery = identificationViewModelQuery;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Observation = _sightingViewModelQuery.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string projectId = "")
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            if (!string.IsNullOrWhiteSpace(projectId))
            {
                var project = _documentSession.Load<Project>(projectId);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return new HttpUnauthorizedResult(); // TODO: Probably should return a soft user error suggesting user joins project
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelQuery.BuildCreateObservation(string.Empty, projectId);
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.ProjectsSelectList = GetProjectsSelectList(projectId);
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel, 
                "observations", 
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            var observation = _documentSession.Load<Observation>(observationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelQuery.BuildSighting(observationId);
            viewModel.CategorySelectList = GetCategorySelectList(observationId);
            viewModel.ProjectsSelectList = GetProjectsSelectList(observation.Groups.Where(x => x.Group.GroupType == "project").Select(x => x.Group.Id).ToArray());
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel,
                "observations",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelQuery.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "delete");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationUpdateInput createInput, IdentificationUpdateInput identificationCreateInput, SightingNoteUpdateInput sightingNoteCreateInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            Observation observation = null;

            if (ModelState.IsValid)
            {
                var key = string.IsNullOrWhiteSpace(createInput.Key) ? Guid.NewGuid().ToString() : createInput.Key;

                observation = _messageBus.Send<ObservationCreateCommand, Observation>(new ObservationCreateCommand()
                    {
                        Key = key,
                        Title = createInput.Title,
                        Latitude = createInput.Latitude,
                        Longitude = createInput.Longitude,
                        Address = createInput.Address,
                        AnonymiseLocation = createInput.AnonymiseLocation,
                        Category = createInput.Category,
                        ObservedOn = createInput.ObservedOn,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Projects = createInput.ProjectIds,
                        Media = createInput.Media.Select(x => new ObservationMediaUpdateCommand()
                            {
                                MediaResourceId = x.MediaResourceId,
                                Key = x.Key,
                                Description = x.Description,
                                Licence = x.Licence,
                                IsPrimaryMedia = x.IsPrimaryMedia
                            })
                    });

                if (identificationCreateInput.NewSightingIdentification)
                {
                    _messageBus.Send(
                        new IdentificationCreateCommand()
                            {
                                SightingId = observation.Id,
                                UserId = _userContext.GetAuthenticatedUserId(),
                                Comments = identificationCreateInput.IdentificationComments ?? string.Empty,
                                IsCustomIdentification = identificationCreateInput.IsCustomIdentification,
                                Taxonomy = identificationCreateInput.Taxonomy ?? string.Empty,
                                Category = identificationCreateInput.Category ?? string.Empty,
                                Kingdom = identificationCreateInput.Kingdom ?? string.Empty,
                                Phylum = identificationCreateInput.Phylum ?? string.Empty,
                                Class = identificationCreateInput.Class ?? string.Empty,
                                Order = identificationCreateInput.Order ?? string.Empty,
                                Family = identificationCreateInput.Family ?? string.Empty,
                                Genus = identificationCreateInput.Genus ?? string.Empty,
                                Species = identificationCreateInput.Species ?? string.Empty,
                                Subspecies = identificationCreateInput.Subspecies ?? string.Empty,
                                CommonGroupNames = identificationCreateInput.CommonGroupNames ?? new string[] {},
                                CommonNames = identificationCreateInput.CommonNames ?? new string[] {},
                                Synonyms = identificationCreateInput.Synonyms ?? new string[] {}
                            });
                }

                if (sightingNoteCreateInput.NewSightingNote)
                {
                    _messageBus.Send(
                        new SightingNoteCreateCommand()
                            {
                                SightingId = observation.Id,
                                UserId = _userContext.GetAuthenticatedUserId(),
                                Descriptions = sightingNoteCreateInput.Descriptions ?? new Dictionary<string, string>(),
                                Tags = sightingNoteCreateInput.Tags ?? string.Empty,
                                Comments = sightingNoteCreateInput.NoteComments ?? string.Empty
                            });
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Observation = new
                {
                    Id = observation != null ? observation.Id : createInput.Id,
                    createInput.Address,
                    createInput.AnonymiseLocation,
                    createInput.Category,
                    createInput.Key,
                    createInput.Latitude,
                    createInput.Longitude,
                    createInput.Media,
                    createInput.ObservedOn,
                    createInput.ProjectIds,
                    createInput.Title
                };

            if (identificationCreateInput.NewSightingIdentification)
            {
                viewModel.Identification = identificationCreateInput;
            }

            if (sightingNoteCreateInput.NewSightingNote)
            {
                viewModel.SightingNote = sightingNoteCreateInput;
            }

            return RestfulResult(
                viewModel,
                "observations",
                "create");
        }

        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput updateInput)
        {
            string observationId = VerbosifyId<Observation>(updateInput.Id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new ObservationUpdateCommand
                        {
                            Id = observationId,
                            Title = updateInput.Title,
                            Latitude = updateInput.Latitude,
                            Longitude = updateInput.Longitude,
                            Address = updateInput.Address,
                            AnonymiseLocation = updateInput.AnonymiseLocation,
                            Category = updateInput.Category,
                            ObservedOn = updateInput.ObservedOn,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Projects = updateInput.ProjectIds,
                            Media = updateInput.Media.Select(x => new ObservationMediaUpdateCommand()
                                {
                                    MediaResourceId = x.MediaResourceId,
                                    Description = x.Description,
                                    Licence = x.Licence,
                                    IsPrimaryMedia = x.IsPrimaryMedia
                                })
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Observation = updateInput;

            return RestfulResult(
                viewModel,
                "observations",
                "update");
        }

        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ObservationDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateIdentificationForm(string id)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, id))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.Identification = _identificationViewModelQuery.BuildCreateIdentification(observationId);
            viewModel.Sighting = _sightingViewModelQuery.BuildSighting(observationId);
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel,
                "identifications",
                "createidentification");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNoteForm(string id)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, id))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.SightingNote = _sightingNoteViewModelQuery.BuildCreateSightingNote(observationId);
            viewModel.Sighting = _sightingViewModelQuery.BuildSighting(observationId);
            viewModel.DescriptionTypesSelectList = GetDescriptionTypesSelectList();
            viewModel.DescriptionTypes = GetDescriptionTypes();
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "createnote");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateIdentificationForm(string id, int identificationId)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateSightingNote))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.Identification = _identificationViewModelQuery.BuildUpdateIdentification(observationId, identificationId);
            viewModel.Sighting = _sightingViewModelQuery.BuildSighting(observationId);
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel,
                "identifications",
                "updateidentification");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateNoteForm(string id, int sightingNoteId)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateSightingNote))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var observationId = VerbosifyId<Observation>(id);

            dynamic viewModel = new ExpandoObject();

            viewModel.SightingNote = _sightingNoteViewModelQuery.BuildUpdateSightingNote(observationId, sightingNoteId);
            viewModel.Sighting = _sightingViewModelQuery.BuildSighting(observationId);
            viewModel.DescriptionTypesSelectList = GetDescriptionTypesSelectList();
            viewModel.DescriptionTypes = GetDescriptionTypes();
            viewModel.CategorySelectList = GetCategorySelectList();
            viewModel.Categories = Categories.GetAll();

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "updatenote");
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNote(SightingNoteUpdateInput createInput)
        {
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, createInput.SightingId))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new SightingNoteCreateCommand()
                        {
                            SightingId = createInput.SightingId,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Descriptions = createInput.Descriptions ?? new Dictionary<string, string>(),
                            Tags = createInput.Tags ?? string.Empty,
                            Comments = createInput.NoteComments ?? string.Empty
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Note = createInput;

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "createnote");
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateIdentification(IdentificationUpdateInput createInput)
        {
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, createInput.SightingId))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new IdentificationCreateCommand()
                        {
                            SightingId = createInput.SightingId,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Comments = createInput.IdentificationComments ?? string.Empty,
                            IsCustomIdentification = createInput.IsCustomIdentification,
                            Taxonomy = createInput.Taxonomy ?? string.Empty,
                            Category = createInput.Category ?? string.Empty,
                            Kingdom = createInput.Kingdom ?? string.Empty,
                            Phylum = createInput.Phylum ?? string.Empty,
                            Class = createInput.Class ?? string.Empty,
                            Order = createInput.Order ?? string.Empty,
                            Family = createInput.Family ?? string.Empty,
                            Genus = createInput.Genus ?? string.Empty,
                            Species = createInput.Species ?? string.Empty,
                            Subspecies = createInput.Subspecies ?? string.Empty,
                            CommonGroupNames = createInput.CommonGroupNames ?? new string[] {},
                            CommonNames = createInput.CommonNames ?? new string[] {},
                            Synonyms = createInput.Synonyms ?? new string[] {}
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Identification = createInput;

            return RestfulResult(
                viewModel,
                "identifications",
                "createidentification");
        }

        [HttpPut]
        [Authorize]
        public ActionResult UpdateNote(SightingNoteUpdateInput updateInput)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, updateInput.Id))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new SightingNoteUpdateCommand()
                        {
                            Id = updateInput.Id.Value,
                            SightingId = updateInput.SightingId,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Descriptions = updateInput.Descriptions ?? new Dictionary<string, string>(),
                            Tags = updateInput.Tags ?? string.Empty,
                            Comments = updateInput.NoteComments ?? string.Empty
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Note = updateInput;

            return RestfulResult(
                viewModel,
                "sightingnotes",
                "updatenote");
        }

        [HttpPut]
        [Authorize]
        public ActionResult UpdateIdentification(IdentificationUpdateInput updateInput)
        {
            // TODO: Check permission to edit this note
            //if (!_userContext.HasGroupPermission<Observation>(PermissionNames.CreateSightingNote, updateInput.Id))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new IdentificationUpdateCommand()
                        {
                            Id = updateInput.Id.Value,
                            SightingId = updateInput.SightingId,
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Comments = updateInput.IdentificationComments,
                            IsCustomIdentification = updateInput.IsCustomIdentification,
                            Taxonomy = updateInput.Taxonomy ?? string.Empty,
                            Category = updateInput.Category ?? string.Empty,
                            Kingdom = updateInput.Kingdom ?? string.Empty,
                            Phylum = updateInput.Phylum ?? string.Empty,
                            Class = updateInput.Class ?? string.Empty,
                            Order = updateInput.Order ?? string.Empty,
                            Family = updateInput.Family ?? string.Empty,
                            Genus = updateInput.Genus ?? string.Empty,
                            Species = updateInput.Species ?? string.Empty,
                            Subspecies = updateInput.Subspecies ?? string.Empty,
                            CommonGroupNames = updateInput.CommonGroupNames ?? new string[] {},
                            CommonNames = updateInput.CommonNames ?? new string[] {},
                            Synonyms = updateInput.Synonyms ?? new string[] {}
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Identification = updateInput;

            return RestfulResult(
                viewModel,
                "identifications",
                "updateidentification");
        }

        public ActionResult CategoryList()
        {
            return RestfulResult(Categories.GetAll(), "", "");
        }

        public ActionResult DescriptionTypes()
        {
            return RestfulResult(GetDescriptionTypes(), "", "");
        }

        private object GetCategorySelectList(string observationId = "", string category = "")
        {
            if (!string.IsNullOrWhiteSpace(observationId))
            {
                category = _documentSession.Load<Observation>(observationId).Category;
            }

            return Categories.GetSelectList(category);
        }

        private IEnumerable GetProjectsSelectList(params string[] projectIds)
        {
            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .Single()
                .Projects
                .Select(x => new
                {
                    Text = x.Name,
                    Value = x.Id,
                    Selected = projectIds.Any(y => y == x.Id)
                });
        }

        private IEnumerable GetDescriptionTypesSelectList()
        {
            return new List<object>
                {
                    new
                        {
                            Text = "Physical Description",
                            Value = "physicaldescription",
                            Selected = false
                        },
                    new
                        {
                            Text = "Similar Species",
                            Value = "similarspecies",
                            Selected = false
                        },
                    new
                        {
                            Text = "Distribution",
                            Value = "distribution",
                            Selected = false
                        },
                    new
                        {
                            Text = "Habitat",
                            Value = "habitat",
                            Selected = false
                        },
                    new
                        {
                            Text = "Seasonal Variation",
                            Value = "seasonalvariation",
                            Selected = false
                        },
                    new
                        {
                            Text = "Behaviour",
                            Value = "behaviour",
                            Selected = false
                        },
                    new
                        {
                            Text = "Food",
                            Value = "food",
                            Selected = false
                        },
                    new
                        {
                            Text = "Life Cycle",
                            Value = "lifecycle",
                            Selected = false
                        },
                    new
                        {
                            Text = "Conservation Status",
                            Value = "conservationstatus",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Name",
                            Value = "indigenousname",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Use",
                            Value = "indigenoususe",
                            Selected = false
                        },
                    new
                        {
                            Text = "Indigenous Stories",
                            Value = "indigenousstories",
                            Selected = false
                        },
                    new
                        {
                            Text = "General Details",
                            Value = "other",
                            Selected = false
                        }
                };
        }

        private IEnumerable GetDescriptionTypes()
        {
            return new List<object>
                {
                    new {
                        Id = "physicaldescription",
                        Group = "lookslike",
                        GroupLabel = "Looks Like",
                        Name = "Physical Description",
                        Description = "The physical characteristics of the species in the sighting"
                    },
                    new {
                        Id = "similarspecies",
                        Group = "lookslike",
                        GroupLabel = "Looks Like",
                        Name = "Similar Species",
                        Description = "How the species sighting is similar to other species"
                    },
                    new {
                        Id = "distribution",
                        Group = "wherefound",
                        GroupLabel = "Where Found",
                        Name = "Distribution",
                        Description = "The geographic distribution of the species in the sighting"
                    },
                    new {
                        Id = "habitat",
                        Group = "wherefound",
                        GroupLabel = "Where Found",
                        Name = "Habitat",
                        Description = "The habitat of the species in the sighting"
                    },
                    new {
                        Id = "seasonalvariation",
                        Group = "wherefound",
                        GroupLabel = "Where Found",
                        Name = "Seasonal Variation",
                        Description = "Any seasonal variation of the species in the sighting"
                    },
                    new {
                        Id = "conservationstatus",
                        Group = "wherefound",
                        GroupLabel = "Where Found",
                        Name = "Conservation Status",
                        Description = "The conservation status of the species in the sighting"
                    },
                    new {
                        Id = "behaviour",
                        Group = "whatitdoes",
                        GroupLabel = "What It Does",
                        Name = "Behaviour",
                        Description = "Any behaviour of a species in the sighting"
                    },
                    new {
                        Id = "food",
                        Group = "whatitdoes",
                        GroupLabel = "What It Does",
                        Name = "Food",
                        Description = "The feeding chracteristics of the species in the sighting"
                    },
                    new {
                        Id = "lifecycle",
                        Group = "whatitdoes",
                        GroupLabel = "What It Does",
                        Name = "Life Cycle",
                        Description = "The life cycle stage or breeding charatcertic of the species in the sighting"
                    },
                    new {
                        Id = "indigenouscommonnames",
                        Group = "cultural",
                        GroupLabel = "What It Does",
                        Name = "Indigenous Common Names",
                        Description = "Any indigenous common names associated with the sighting"
                    },
                    new {
                        Id = "indigenoususage",
                        Group = "cultural",
                        GroupLabel = "Cultural",
                        Name = "Usage in Indigenous Culture",
                        Description = "Any special usage in indigenous cultures of the species in the sighting"
                    },
                    new {
                        Id = "traditionalstories",
                        Group = "cultural",
                        GroupLabel = "Cultural",
                        Name = "Traditional Stories",
                        Description = "Any traditional stories associated with the species in the sighting"
                    },
                    new {
                        Id = "general",
                        Group = "other",
                        GroupLabel = "Other",
                        Name = "General Details",
                        Description = "Any other general details"
                    }
                };
        }

        #endregion
    }
}