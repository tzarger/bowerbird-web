﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using System;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ProjectsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IProjectsViewModelBuilder _viewModelBuilder;

        #endregion

        #region Constructors

        public ProjectsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IProjectsViewModelBuilder projectsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectsViewModelBuilder, "projectsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = projectsViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            ViewBag.Model = _viewModelBuilder.BuildIndex(pagingInput);
            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult Observations(PagingInput pagingInput)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult About()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.ProjectList = _viewModelBuilder.BuildList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return Json(_viewModelBuilder.BuildItem(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return Json(_viewModelBuilder.BuildList(pagingInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, idInput.Id))
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

            ViewBag.Project = _viewModelBuilder.BuildItem(idInput);

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

            ViewBag.Project = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Delete);
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
                new ProjectDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}