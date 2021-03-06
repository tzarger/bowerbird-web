﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia

*/

using System;
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
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private Mock<IProjectsViewModelBuilder> _mockProjectsViewModelBuilder;
        private Mock<ITeamsViewModelBuilder> _mockTeamsViewModelBuilder;
        private Mock<IStreamItemsViewModelBuilder> _mockStreamItemsViewModelBuilder;
        private Mock<IObservationsViewModelBuilder> _mockObservationsViewModelBuilder;
        private Mock<IPostsViewModelBuilder> _mockPostsViewModelBuilder;
        private Mock<IReferenceSpeciesViewModelBuilder> _mockReferenceSpeciesViewModelBuilder;
        private ProjectsController _controller;
        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _mockProjectsViewModelBuilder = new Mock<IProjectsViewModelBuilder>();
            _mockTeamsViewModelBuilder = new Mock<ITeamsViewModelBuilder>();
            _mockStreamItemsViewModelBuilder = new Mock<IStreamItemsViewModelBuilder>();
            _mockObservationsViewModelBuilder = new Mock<IObservationsViewModelBuilder>();
            _mockPostsViewModelBuilder = new Mock<IPostsViewModelBuilder>();
            _mockReferenceSpeciesViewModelBuilder = new Mock<IReferenceSpeciesViewModelBuilder>();

            using (var documentSession = _documentStore.OpenSession())
            {
                _controller = new ProjectsController(
                    _mockCommandProcessor.Object,
                    _mockUserContext.Object,
                    _mockProjectsViewModelBuilder.Object,
                    _mockTeamsViewModelBuilder.Object,
                    _mockStreamItemsViewModelBuilder.Object,
                    _mockObservationsViewModelBuilder.Object,
                    _mockPostsViewModelBuilder.Object,
                    _mockReferenceSpeciesViewModelBuilder.Object,
                    documentSession
                    );
            }
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
        public void Project_List_As_Json()
        {
            const int page = 1;
            const int pageSize = 10;

            var projects = new List<Project>();

            using (var session = _documentStore.OpenSession())
            {
                for (var i = 0; i < 15; i++)
                {
                    var project = FakeObjects.TestProjectWithId(i.ToString());
                    projects.Add(project);
                    session.Store(project);
                }

                session.SaveChanges();
            }

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            //Assert.IsInstanceOf<ProjectList>(jsonResult.Data);
            //var jsonData = jsonResult.Data as ProjectList;

            //Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Projects.PagedListItems.Count());
            //Assert.AreEqual(projects.Count, jsonData.Projects.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Stream_As_ViewModel()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(project);
                session.Store(user);
                session.Store(team);

                //var groupAssociation = new GroupAssociation(
                //    team,
                //    project,
                //    user,
                //    FakeValues.CreatedDateTime);

                //session.Store(groupAssociation);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Stream(new PagingInput() { Id = project.Id });

            //Assert.IsInstanceOf<ProjectIndex>(_controller.ViewData.Model);

            //var viewModel = _controller.ViewData.Model as ProjectIndex;

            //Assert.IsNotNull(viewModel);
            //Assert.AreEqual(viewModel.Project, project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Stream_As_Json()
        {
            var team = FakeObjects.TestTeamWithId("1");
            var user = FakeObjects.TestUserWithId();

            var project = new Project(
                user,
                FakeValues.Name,
                FakeValues.Description,
                FakeValues.Website,
                null,
                DateTime.UtcNow
                );

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.Store(project);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Stream(new PagingInput() { Id = project.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            //Assert.IsInstanceOf<ProjectIndex>(jsonResult.Data);

            //var jsonData = jsonResult.Data as ProjectIndex;

            //Assert.IsNotNull(jsonData);
            //Assert.AreEqual(project, jsonData.Project);
            //Assert.AreEqual(team, jsonData.Team);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Create_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new ProjectCreateInput()
                                                {
                                                    Description = FakeValues.Description,
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Create_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new ProjectCreateInput()
                                                {
                                                    Description = FakeValues.Description,
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Create_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new ProjectCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Update_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new ProjectUpdateInput()
                                                {
                                                    ProjectId = FakeValues.KeyString,
                                                    Description = FakeValues.Description,
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Update_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new ProjectUpdateInput()
                                                {
                                                    ProjectId = FakeValues.KeyString,
                                                    Description = FakeValues.Description,
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Update_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new ProjectUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_With_Success()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion
    }
}