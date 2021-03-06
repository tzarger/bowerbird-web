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
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class ObservationCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
            
        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;             
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId();
            var userProject = FakeObjects.TestUserProjectWithId();
            var createdDateTime = DateTime.UtcNow;

            Observation newValue = null;

            var command = new ObservationCreateCommand()
            {
                UserId = user.Id,
                Address = FakeValues.Address,
                Latitude = FakeValues.Latitude,
                Longitude = FakeValues.Longitude,
                Media = new List<Tuple<string,string,string>>(){new Tuple<string, string, string>(imageMediaResource.Id, FakeValues.Message,FakeValues.Message)},
                Category = FakeValues.Category,
                ObservedOn = createdDateTime,
                Title = FakeValues.Title
            };

            using (var session = _store.OpenSession())
            {
                // for polymorphism: see http://ravendb.net/faq/polymorphism
                //_store.Conventions.CustomizeJsonSerializer = serializer => serializer.TypeNameHandling = TypeNameHandling.All;
                
                session.Store(user);
                session.Store(imageMediaResource);
                session.Store(userProject);
                session.SaveChanges();

                var commandHandler = new ObservationCreateCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                newValue = session
                    .Query<Observation>()
                    .FirstOrDefault(x => x.ObservedOn == createdDateTime);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Address, newValue.Address);
            Assert.AreEqual(command.Latitude, newValue.Latitude);
            Assert.AreEqual(command.Longitude, newValue.Longitude);
            Assert.AreEqual(command.Category, newValue.Category);
            Assert.AreEqual(command.ObservedOn, newValue.ObservedOn);
            Assert.AreEqual(command.Title, newValue.Title);
        }

        #endregion
    }
}