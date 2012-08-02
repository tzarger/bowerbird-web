﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Raven.Client;
using System.Collections;
using System.Dynamic;

namespace Bowerbird.Web.Controllers
{
    public class RecordsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public RecordsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IDocumentSession documentSession,
            IPermissionChecker permissionChecker
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionChecker, "permissionChecker");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _documentSession = documentSession;
            _permissionChecker = permissionChecker;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionChecker.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            var viewModel = new
                {
                    Record = _sightingViewModelBuilder.BuildSighting(recordId)
                };

            return RestfulResult(
                viewModel,
                "records",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string id = null)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                var project = _documentSession.Load<Project>(id);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return HttpUnauthorized();
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = _sightingViewModelBuilder.BuildNewRecord(id);
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "records", 
                "create", 
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionChecker.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            var record = _sightingViewModelBuilder.BuildSighting(recordId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = record;
            viewModel.Categories = GetCategories(recordId);

            return RestfulResult(
                viewModel,
                "records",
                "update", 
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionChecker.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = _sightingViewModelBuilder.BuildSighting(recordId);

            return RestfulResult(
                viewModel,
                "records",
                "delete", 
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(RecordCreateInput createInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new RecordCreateCommand()
                    {
                        Latitude = createInput.Latitude,
                        Longitude = createInput.Longitude,
                        AnonymiseLocation = createInput.AnonymiseLocation,
                        Category = createInput.Category,
                        ObservedOn = createInput.ObservedOn,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Projects = createInput.Projects
                    });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(RecordUpdateInput updateInput)
        {
            string recordId = VerbosifyId<Record>(updateInput.Id);

            if (!_permissionChecker.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Record>(PermissionNames.UpdateObservation, recordId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new RecordUpdateCommand
                {
                    Id = recordId,
                    Latitude = updateInput.Latitude,
                    Longitude = updateInput.Longitude,
                    AnonymiseLocation = updateInput.AnonymiseLocation,
                    Category = updateInput.Category,
                    ObservedOn = updateInput.ObservedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Projects = updateInput.Projects
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionChecker.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Record>(PermissionNames.UpdateObservation, recordId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new RecordDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private IEnumerable GetCategories(string recordId = "")
        {
            var category = string.Empty;

            if (!string.IsNullOrWhiteSpace(recordId))
            {
                category = _documentSession.Load<Record>(recordId).Category;
            }

            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .Select(x => new
                   {
                       Text = x,
                       Value = x,
                       Selected = x == category
                   });
        }

        #endregion
    }
}