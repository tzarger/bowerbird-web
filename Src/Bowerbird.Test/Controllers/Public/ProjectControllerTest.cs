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
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Public
{
    [TestFixture]
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private ProjectController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _controller = new ProjectController(_documentStore.OpenSession());
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_List_Returns_Json_Success()
        {
            var result = _controller.List(null, null, null);

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_NonAjAxCall_Returns_ProjectIndex_ViewModel_With_Project_Having_Observations_And_Team()
        {
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();
            }

            _controller.Index(new IdInput() { Id = FakeValues.KeyString.PrependWith("projects/") });

            Assert.IsInstanceOf<ProjectIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ProjectIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Project, project);
            Assert.IsNotNull(viewModel.Project.Team);
            Assert.AreEqual(viewModel.Project.Team.Name, team.Name);
            Assert.IsTrue(viewModel.Observations.Contains(observation1));
            Assert.IsTrue(viewModel.Observations.Contains(observation2));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_AjAxCall_Returns_ProjectIndex_ViewModel_With_Project_Having_Observations_And_Team()
        {
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = FakeValues.KeyString.PrependWith("projects/") });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ProjectIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as ProjectIndex;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(jsonData.Project, project);
            Assert.IsNotNull(jsonData.Project.Team);
            Assert.AreEqual(jsonData.Project.Team.Name, project.Team.Name);
            Assert.IsTrue(jsonData.Observations.Contains(observation1));
            Assert.IsTrue(jsonData.Observations.Contains(observation2));
        }

        #endregion
    }
}