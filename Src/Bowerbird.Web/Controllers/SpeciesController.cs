﻿///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Web.Mvc;
//using Bowerbird.Core.Commands;
//using Bowerbird.Core.Config;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Web.Builders;
//using Bowerbird.Web.Config;
//using Bowerbird.Web.ViewModels;

//namespace Bowerbird.Web.Controllers
//{
//    [Restful]
//    public class SpeciesController : ControllerBase
//    {
//        #region Fields

//        private readonly IMessageBus _messageBus;
//        private readonly IUserContext _userContext;
//        private readonly ISpeciesViewModelBuilder _speciesViewModelBuilder;

//        #endregion

//        #region Constructors

//        public SpeciesController(
//            IMessageBus messageBus,
//            IUserContext userContext,
//            ISpeciesViewModelBuilder speciesViewModelBuilder
//        )
//        {
//            Check.RequireNotNull(messageBus, "messageBus");
//            Check.RequireNotNull(userContext, "userContext");
//            Check.RequireNotNull(speciesViewModelBuilder, "speciesViewModelBuilder");

//            _messageBus = messageBus;
//            _userContext = userContext;
//            _speciesViewModelBuilder = speciesViewModelBuilder;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult Index(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            ViewBag.Species = _speciesViewModelBuilder.BuildSpecies(idInput);

//            return View(Form.Index);
//        }

//        [HttpGet]
//        public ActionResult Explore(PagingInput pagingInput)
//        {
//            Check.RequireNotNull(pagingInput, "pagingInput");

//            ViewBag.SpeciesList = _speciesViewModelBuilder.BuildSpeciesList(pagingInput);
            
//            return View(Form.List);
//        }

//        [HttpGet]
//        public ActionResult GetOne(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            return new JsonNetResult(_speciesViewModelBuilder.BuildSpecies(idInput));
//        }

//        [HttpGet]
//        public ActionResult GetMany(PagingInput pagingInput)
//        {
//            Check.RequireNotNull(pagingInput, "pagingInput");

//            return new JsonNetResult(_speciesViewModelBuilder.BuildSpeciesList(pagingInput));
//        }

//        [HttpGet]
//        [Authorize]
//        public ActionResult CreateForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            if (!_userContext.HasGroupPermission(PermissionNames.CreateSpecies, idInput.Id ?? Constants.AppRootId))
//            {
//                return HttpUnauthorized();
//            }

//            return View(Form.Create);
//        }

//        [HttpGet]
//        [Authorize]
//        public ActionResult UpdateForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            if (!_userContext.HasGroupPermission(PermissionNames.UpdateSpecies, idInput.Id))
//            {
//                return HttpUnauthorized();
//            }

//            ViewBag.Species = _speciesViewModelBuilder.BuildSpecies(idInput);

//            return View(Form.Update);
//        }

//        [HttpGet]
//        [Authorize]
//        public ActionResult DeleteForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            if (!_userContext.HasGroupPermission(PermissionNames.DeleteSpecies, idInput.Id))
//            {
//                return HttpUnauthorized();
//            }

//            ViewBag.Species = _speciesViewModelBuilder.BuildSpecies(idInput);

//            return View(Form.Delete);
//        }

//        [Transaction]
//        [HttpPost]
//        [Authorize]
//        public ActionResult Create(SpeciesCreateInput createInput)
//        {
//            Check.RequireNotNull(createInput, "createInput");

//            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateSpecies))
//            {
//                return HttpUnauthorized();
//            }

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new SpeciesCreateCommand()
//                {
//                    Kingdom = createInput.Kingdom,
//                    Group = createInput.Group,
//                    CommonNames = createInput.CommonNames,
//                    Taxonomy = createInput.Taxonomy,
//                    Order = createInput.Order,
//                    Family = createInput.Family,
//                    GenusName = createInput.GenusName,
//                    SpeciesName = createInput.SpeciesName,
//                    ProposedAsNewSpecies = createInput.ProposedAsNewSpecies,
//                    UserId = _userContext.GetAuthenticatedUserId(),
//                    CreatedOn = createInput.CreatedOn
//                });

//            return JsonSuccess();
//        }

//        [Transaction]
//        [HttpPut]
//        [Authorize]
//        public ActionResult Update(SpeciesUpdateInput updateInput)
//        {
//            Check.RequireNotNull(updateInput, "updateInput");

//            if (!_userContext.HasAppRootPermission(PermissionNames.UpdateSpecies))
//            {
//                return HttpUnauthorized();
//            }

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new SpeciesUpdateCommand
//                {
//                    Kingdom = updateInput.Kingdom,
//                    Group = updateInput.Group,
//                    CommonNames = updateInput.CommonNames,
//                    Taxonomy = updateInput.Taxonomy,
//                    Order = updateInput.Order,
//                    Family = updateInput.Family,
//                    GenusName = updateInput.GenusName,
//                    SpeciesName = updateInput.SpeciesName,
//                    ProposedAsNewSpecies = updateInput.ProposedAsNewSpecies,
//                    UserId = _userContext.GetAuthenticatedUserId(),
//                    UpdatedOn = updateInput.UpdatedOn
//                });

//            return JsonSuccess();
//        }

//        [Transaction]
//        [HttpDelete]
//        [Authorize]
//        public ActionResult Delete(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            //if (!_userContext.HasGroupPermission<Species>(PermissionNames.DeleteSpecies, idInput.Id))
//            //{
//            //    return HttpUnauthorized();
//            //}

//            if (!ModelState.IsValid)
//            {
//                return JsonFailed();
//            }

//            _commandProcessor.Process(
//                new SpeciesDeleteCommand
//                {
//                    Id = idInput.Id,
//                    UserId = _userContext.GetAuthenticatedUserId()
//                });

//            return JsonSuccess();
//        }

//        #endregion
//    }
//}