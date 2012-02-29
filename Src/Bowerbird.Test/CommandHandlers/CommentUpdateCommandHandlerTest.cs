/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class CommentUpdateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.InMemoryDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void CommentUpdateCommandHandler_Handle()
        {
            var observation = FakeObjects.TestObservationWithId("1");
            var user = FakeObjects.TestUserWithId();

            Comment updated = null;

            observation.AddComment(FakeValues.Message, user, FakeValues.CreatedDateTime);

            using (var session = _store.OpenSession())
            {
                session.Store(observation);
                session.Store(user);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var command = new CommentUpdateCommand()
                {
                    ContributionId = observation.Id,
                    Id = observation.Comments.ToList()[0].Id,
                    Comment = FakeValues.Message.AppendWith("-changed"),
                    UpdatedOn = FakeValues.ModifiedDateTime,
                    UserId = user.Id
                };

                var commandHandler = new CommentUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                updated = session.Load<Observation>(observation.Id).Comments.FirstOrDefault();
            }

            Assert.IsNotNull(updated);
            Assert.AreEqual(FakeValues.Message.AppendWith("-changed"), updated.Message);
        }

        #endregion
    }
}