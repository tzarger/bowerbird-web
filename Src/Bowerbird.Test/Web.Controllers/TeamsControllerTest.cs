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
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class TeamsControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private Mock<ITeamsViewModelBuilder> _mockTeamsViewModelBuilder;
        private Mock<IStreamItemsViewModelBuilder> _mockStreamItemsViewModelBuilder;
        private Mock<IProjectsViewModelBuilder> _mockProjectsViewModelBuilder;
        private Mock<IPostsViewModelBuilder> _mockPostsViewModelBuilder;
        private Mock<IMemberViewModelBuilder> _mockMemberViewModelBuilder;
        private Mock<IReferenceSpeciesViewModelBuilder> _mockReferenceSpeciesViewModelBuilder;
        private IDocumentStore _documentStore;
        private TeamsController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _mockTeamsViewModelBuilder = new Mock<ITeamsViewModelBuilder>();
            _mockStreamItemsViewModelBuilder = new Mock<IStreamItemsViewModelBuilder>();
            _mockProjectsViewModelBuilder = new Mock<IProjectsViewModelBuilder>();
            _mockPostsViewModelBuilder = new Mock<IPostsViewModelBuilder>();
            _mockMemberViewModelBuilder = new Mock<IMemberViewModelBuilder>();
            _mockReferenceSpeciesViewModelBuilder = new Mock<IReferenceSpeciesViewModelBuilder>();

            _documentStore = DocumentStoreHelper.StartRaven();

            _controller = new TeamsController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _mockTeamsViewModelBuilder.Object,
                _mockStreamItemsViewModelBuilder.Object,
                _mockProjectsViewModelBuilder.Object,
                _mockPostsViewModelBuilder.Object,
                _mockMemberViewModelBuilder.Object,
                _mockReferenceSpeciesViewModelBuilder.Object
                );
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_List_As_Json()
        {
            var organisation = FakeObjects.TestOrganisationWithId();
            var user = FakeObjects.TestUserWithId();

            const int page = 1;
            const int pageSize = 10;

            var teams = new List<Team>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(organisation);
                session.Store(user);

                for (var i = 0; i < 15; i++)
                {
                    var team = FakeObjects.TestTeamWithId(i.ToString());
                    teams.Add(team);
                    session.Store(team);
                }

                session.SaveChanges();
            }

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            //Assert.IsInstanceOf<TeamList>(jsonResult.Data);
            //var jsonData = jsonResult.Data as TeamList;

            //Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Teams.PagedListItems.Count());
            //Assert.AreEqual(teams.Count, jsonData.Teams.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Stream_As_ViewModel()
        {
            var team = FakeObjects.TestTeamWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Stream(new PagingInput() { Id = team.Id });

            //Assert.IsInstanceOf<TeamIndex>(_controller.ViewData.Model);

            //var viewModel = _controller.ViewData.Model as TeamIndex;

            //Assert.IsNotNull(viewModel);
            //Assert.AreEqual(viewModel.Team, team);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Stream_As_Json()
        {
            var team = FakeObjects.TestTeamWithId();
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Stream(new PagingInput() { Id = team.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            //Assert.IsInstanceOf<TeamIndex>(jsonResult.Data);

            //var jsonData = jsonResult.Data as TeamIndex;
            //Assert.IsNotNull(jsonData);
            //Assert.AreEqual(jsonData.Team, team);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Create_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new TeamCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Create_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new TeamCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Create_With_HttpUnAuthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new TeamCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Update_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new TeamUpdateInput()
            {
                Id = FakeValues.KeyString,
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Update_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new TeamUpdateInput()
            {
                Id = FakeValues.KeyString,
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Update_With_HttpUnAuthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new TeamUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Delete_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Delete_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Delete_With_HttpUnAuthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new TeamUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion 
    }
}