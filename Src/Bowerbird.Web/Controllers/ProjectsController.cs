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
using System.Dynamic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Config;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Config;
using System;
using System.Linq;
using Bowerbird.Web.Infrastructure;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Controllers
{
    public class ProjectsController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IProjectViewModelQuery _projectViewModelQuery;
        private readonly IActivityViewModelQuery _activityViewModelQuery;
        private readonly IPostViewModelQuery _postViewModelQuery;
        private readonly ISightingViewModelQuery _sightingViewModelQuery;
        private readonly IUserViewModelQuery _userViewModelQuery;
        private readonly IPermissionManager _permissionManager;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectsController(
            IMessageBus messageBus,
            IUserContext userContext,
            IProjectViewModelQuery projectViewModelQuery,
            ISightingViewModelQuery sightingViewModelQuery,
            IActivityViewModelQuery activityViewModelQuery,
            IPostViewModelQuery postViewModelQuery,
            IUserViewModelQuery userViewModelQuery,
            IPermissionManager permissionManager,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectViewModelQuery, "projectViewModelQuery");
            Check.RequireNotNull(sightingViewModelQuery, "sightingViewModelQuery");
            Check.RequireNotNull(activityViewModelQuery, "activityViewModelQuery");
            Check.RequireNotNull(postViewModelQuery, "postViewModelQuery");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _projectViewModelQuery = projectViewModelQuery;
            _sightingViewModelQuery = sightingViewModelQuery;
            _activityViewModelQuery = activityViewModelQuery;
            _postViewModelQuery = postViewModelQuery;
            _userViewModelQuery = userViewModelQuery;
            _permissionManager = permissionManager;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Sightings(string id, SightingsQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (queryInput.View.ToLower() == "thumbnails")
            {
                queryInput.PageSize = 15;
            }

            if (queryInput.View.ToLower() == "details")
            {
                queryInput.PageSize = 10;
            }

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            queryInput.Category = queryInput.Category ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(queryInput.Category) && !Categories.IsValidCategory(queryInput.Category))
            {
                queryInput.Category = string.Empty;
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            queryInput.Taxonomy = queryInput.Taxonomy ?? string.Empty;

            var projectResult = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == projectId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.Sightings = _sightingViewModelQuery.BuildGroupSightingList(projectId, queryInput);
            viewModel.UserCountDescription = "Member" + (projectResult.UserCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (projectResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (projectResult.PostCount == 1 ? string.Empty : "s");
            viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category);
            viewModel.Categories = Categories.GetAll();
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.View,
                queryInput.Category,
                queryInput.NeedsId,
                queryInput.Query,
                queryInput.Field,
                queryInput.Taxonomy,
                IsThumbnailsView = queryInput.View == "thumbnails",
                IsDetailsView = queryInput.View == "details",
                IsMapView = queryInput.View == "map"
            };
            viewModel.ShowSightings = true;
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Sighting Title",
                            Value = "title",
                            Selected = queryInput.Field.ToLower() == "title"
                        },
                    new
                        {
                            Text = "Descriptions",
                            Value = "descriptions",
                            Selected = queryInput.Field.ToLower() == "descriptions"
                        },
                    new
                        {
                            Text = "Tags",
                            Value = "tags",
                            Selected = queryInput.Field.ToLower() == "tags"
                        },
                    new
                        {
                            Text = "Scientific Name",
                            Value = "scientificname",
                            Selected = queryInput.Field.ToLower() == "scientificname"
                        },
                    new
                        {
                            Text = "Common Name",
                            Value = "commonname",
                            Selected = queryInput.Field.ToLower() == "commonname"
                        }
                };

            return RestfulResult(
                viewModel,
                "projects",
                "sightings");
        }

        [HttpGet]
        public ActionResult Posts(string id, PostsQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            queryInput.PageSize = 10;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            var projectResult = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == projectId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.Posts = _postViewModelQuery.BuildGroupPostList(projectId, queryInput);
            viewModel.UserCountDescription = "Member" + (projectResult.UserCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (projectResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (projectResult.PostCount == 1 ? string.Empty : "s");
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.Query,
                queryInput.Field
            };
            viewModel.ShowPosts = true;
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Title",
                            Value = "title",
                            Selected = queryInput.Field.ToLower() == "title"
                        },
                    new
                        {
                            Text = "Body",
                            Value = "body",
                            Selected = queryInput.Field.ToLower() == "body"
                        }
                };

            return RestfulResult(
                viewModel,
                "projects",
                "posts");
        }

        [HttpGet]
        public ActionResult  Members(string id, UsersQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "a-z";
            }

            var projectResult = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == projectId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.Users = _userViewModelQuery.BuildGroupUserList(projectId, queryInput);
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.Query,
                queryInput.Field
            };
            viewModel.ShowMembers = true;
            viewModel.IsMember = _userContext.IsUserAuthenticated() ? _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId) : false;
            viewModel.UserCountDescription = "Member" + (projectResult.UserCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (projectResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (projectResult.PostCount == 1 ? string.Empty : "s");

            return RestfulResult(
                viewModel,
                "projects",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var projectResult = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == projectId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.ShowAbout = true;
            viewModel.IsMember = _userContext.IsUserAuthenticated() ? _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId) : false;
            viewModel.UserCountDescription = "Member" + (projectResult.UserCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (projectResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (projectResult.PostCount == 1 ? string.Empty : "s");
            viewModel.ProjectAdministrators = _userViewModelQuery.BuildGroupUserList(projectId, "roles/" + RoleNames.ProjectAdministrator);
            viewModel.ActivityTimeseries = CreateActivityTimeseries(projectId);

            return RestfulResult(
                viewModel,
                "projects",
                "about");
        }

        [HttpGet]
        public ActionResult Index(string id, ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var projectResult = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == projectId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.Activities = _activityViewModelQuery.BuildGroupActivityList(projectId, activityInput, pagingInput);
            viewModel.IsMember = _userContext.IsUserAuthenticated() ? _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId) : false;
            viewModel.UserCountDescription = "Member" + (projectResult.UserCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (projectResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (projectResult.PostCount == 1 ? string.Empty : "s");
            viewModel.ShowActivities = true;

            return RestfulResult(
                viewModel,
                "projects",
                "index");
        }

        [HttpGet]
        public ActionResult List(ProjectsQueryInput queryInput)
        {
            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "popular" &&
                queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "popular";
            }

            queryInput.Category = queryInput.Category ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(queryInput.Category) && !Categories.IsValidCategory(queryInput.Category))
            {
                queryInput.Category = string.Empty;
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            dynamic viewModel = new ExpandoObject();
            viewModel.Projects = _projectViewModelQuery.BuildProjectList(queryInput);
            viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category);
            viewModel.Query = new
            {
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.Category,
                queryInput.Query,
                queryInput.Field
            };
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Name",
                            Value = "name",
                            Selected = queryInput.Field.ToLower() == "name"
                        },
                    new
                        {
                            Text = "Description",
                            Value = "description",
                            Selected = queryInput.Field.ToLower() == "description"
                        }
                };

            if (_userContext.IsUserAuthenticated())
            {
                var user = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                    .Single();

                viewModel.ShowProjectExploreWelcome = user.User.CallsToAction.Contains("project-explore-welcome");
            }

            return RestfulResult(
                viewModel,
                "projects",
                "list");    
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildCreateProject();
            viewModel.Create = true;
            viewModel.CategoriesSelectList = Categories.GetSelectList();

            return RestfulResult(
                viewModel,
                "projects",
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateProject, projectId))
            {
                return new HttpUnauthorizedResult();
            }

            var project = _documentSession.Load<Project>(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildUpdateProject(projectId);
            viewModel.Update = true;
            viewModel.CategoriesSelectList = Categories.GetSelectList(project.Categories.ToArray());

            return RestfulResult(
                viewModel,
                "projects",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelQuery.BuildProject(projectId);
            viewModel.Delete = true;

            return RestfulResult(
                viewModel,
                "projects",
                "delete");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateMember(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed(); 
            }

            _messageBus.Send(
                new MemberUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    ModifiedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { "roles/projectmember" }
                });

            return JsonSuccess();
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMember(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            //// TODO: Not sure what this permission check is actually checking???
            //if (!_userContext.HasGroupPermission(PermissionNames.LeaveProject, projectId))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    ModifiedByUserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(ProjectUpdateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new ProjectCreateCommand()
                        {
                            UserId = _userContext.GetAuthenticatedUserId(),
                            Name = createInput.Name,
                            Description = createInput.Description,
                            Website = createInput.Website,
                            AvatarId = createInput.AvatarId,
                            BackgroundId = createInput.BackgroundId,
                            Categories = createInput.Categories
                        });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = createInput;

            return RestfulResult(
                viewModel,
                "projects",
                "create");
        }

        [HttpPut]
        [Authorize]
        public ActionResult Update(ProjectUpdateInput updateInput)
        {
            string projectId = VerbosifyId<Project>(updateInput.Id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, projectId))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new ProjectUpdateCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Id = projectId,
                        Name = updateInput.Name,
                        Description = updateInput.Description,
                        Website = updateInput.Website,
                        AvatarId = updateInput.AvatarId,
                        BackgroundId =  updateInput.BackgroundId,
                        Categories = updateInput.Categories
                    });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = updateInput;

            return RestfulResult(
                viewModel,
                "projects",
                "update");
        }

        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ProjectDeleteCommand
                {
                    Id = projectId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private dynamic CreateActivityTimeseries(string projectId)
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)).Date;
            var toDate = DateTime.UtcNow.Date;

            var result = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .SelectFields<All_Contributions.Result>("ParentContributionId", "SubContributionId", "ParentContributionType", "SubContributionType", "CreatedDateTime")
                .WhereGreaterThan(x => x.CreatedDateTime, fromDate)
                .AndAlso()
                .WhereIn("GroupIds", new [] { projectId })
                .AndAlso()
                .OpenSubclause()
                .WhereIn("ParentContributionType", new[] { "observation", "record", "post" })
                .OrElse()
                .WhereIn("SubContributionType", new[] { "identification", "note", "comment" })
                .CloseSubclause()
                .ToList();

            var contributions = result.Select(x => new
            {
                x.ParentContributionId,
                x.SubContributionId,
                x.ParentContributionType,
                x.SubContributionType,
                x.CreatedDateTime
            })
            .GroupBy(x => x.CreatedDateTime.Date);

            var timeseries = new List<dynamic>();

            for (DateTime dateItem = fromDate; dateItem <= toDate; dateItem = dateItem.AddDays(1))
            {
                string createdDateFormat;

                if (dateItem == fromDate ||
                    dateItem.Day == 1)
                {
                    createdDateFormat = "d MMM";
                }
                else
                {
                    createdDateFormat = "%d";
                }

                if (contributions.Any(x => x.Key.Date == dateItem.Date))
                {
                    timeseries.Add(contributions
                        .Where(x => x.Key.Date == dateItem.Date)
                        .Select(x => new
                        {
                            CreatedDate = dateItem.ToString(createdDateFormat),
                            SightingCount = x.Count(y => y.ParentContributionType == "observation" || y.ParentContributionType == "record"),
                            IdentificationCount = x.Count(y => y.SubContributionType == "identification"),
                            NoteCount = x.Count(y => y.SubContributionType == "note"),
                            PostCount = x.Count(y => y.ParentContributionType == "post"),
                            CommentCount = x.Count(y => y.SubContributionType == "comment")
                        }
                        ).First());
                }
                else
                {
                    timeseries.Add(new
                    {
                        CreatedDate = dateItem.ToString(createdDateFormat),
                        SightingCount = 0,
                        NoteCount = 0,
                        PostCount = 0,
                        CommentCount = 0
                    });
                }
            }
            return timeseries;
        }

        #endregion
    }
}