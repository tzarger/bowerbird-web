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
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ProjectsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IProjectsViewModelBuilder _projectsViewModelBuilder;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;
        private readonly IObservationsViewModelBuilder _observationsViewModelBuilder;
        private readonly IPostsViewModelBuilder _postsViewModelBuilder;
        private readonly IMemberViewModelBuilder _memberViewModelBuilder;
        private readonly IReferenceSpeciesViewModelBuilder _referenceSpeciesViewModelBuilder;

        #endregion

        #region Constructors

        public ProjectsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IProjectsViewModelBuilder projectsViewModelBuilder,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder,
            IObservationsViewModelBuilder observationsViewModelBuilder,
            IPostsViewModelBuilder postsViewModelBuilder,
            IMemberViewModelBuilder memberViewModelBuilder,
            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectsViewModelBuilder, "projectsViewModelBuilder");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");
            Check.RequireNotNull(observationsViewModelBuilder, "observationsViewModelBuilder");
            Check.RequireNotNull(postsViewModelBuilder, "postsViewModelBuilder");
            Check.RequireNotNull(memberViewModelBuilder, "memberViewModelBuilder");
            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _projectsViewModelBuilder = projectsViewModelBuilder;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
            _observationsViewModelBuilder = observationsViewModelBuilder;
            _postsViewModelBuilder = postsViewModelBuilder;
            _memberViewModelBuilder = memberViewModelBuilder;
            _referenceSpeciesViewModelBuilder = referenceSpeciesViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + pagingInput.Id }),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput)
            };

            ViewBag.PrerenderedView = "projects"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult StreamList(PagingInput pagingInput)
        {
            return new JsonNetResult(_streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput));
        }

        [HttpGet]
        public ActionResult Observations(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + pagingInput.Id }),
                Observations = _observationsViewModelBuilder.BuildProjectObservationList(pagingInput)
            };

            ViewBag.PrerenderedView = "observations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult ReferenceSpecies(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + pagingInput.Id }),
                ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildGroupReferenceSpeciesList(pagingInput)
            };

            ViewBag.PrerenderedView = "referencespecies"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + pagingInput.Id }),
                Posts = _postsViewModelBuilder.BuildGroupPostList(pagingInput)
            };

            ViewBag.PrerenderedView = "posts"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + pagingInput.Id }),
                Members = _memberViewModelBuilder.BuildProjectMemberList(pagingInput)
            };

            ViewBag.PrerenderedView = "members"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult About(IdInput idInput)
        {
            ViewBag.Model = new
            {
                Project = _projectsViewModelBuilder.BuildProject(new IdInput() { Id = "projects/" + idInput.Id }),
            };

            return View(Form.About);
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.Projects = _projectsViewModelBuilder.BuildProjectList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return new JsonNetResult(_projectsViewModelBuilder.BuildProject(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return new JsonNetResult(_projectsViewModelBuilder.BuildProjectList(pagingInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, idInput.Id ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.UpdateProject, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.Project = _projectsViewModelBuilder.BuildProject(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.Project = _projectsViewModelBuilder.BuildProject(idInput);

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.JoinProject, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = idInput.Id,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new []{ RoleNames.ProjectMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.LeaveProject, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new DeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = idInput.Id
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, createInput.Team ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ProjectCreateCommand()
                {
                    Description = createInput.Description,
                    Name = createInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = createInput.Avatar,
                    TeamId = createInput.Team
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput) 
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, updateInput.ProjectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }
            
            _commandProcessor.Process(
                new ProjectUpdateCommand()
                {
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.DeleteProject, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new DeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}