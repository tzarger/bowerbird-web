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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class TeamMemberCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
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
        public void TeamMemberCreateCommandHandler_Creates_TeamMember()
        {
            var user = FakeObjects.TestUserWithId();
            var team = FakeObjects.TestTeamWithId();
            var teamMember = FakeObjects.TestTeamMemberWitId();

            TeamMember newValue = null;

            var command = new TeamMemberCreateCommand()
            {
                TeamId = team.Id,
                UserId = user.Id,
                CreatedByUserId = user.Id,
                Roles = FakeObjects.TestRoles().Select(x => x.Name).ToList()
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.Store(teamMember);

                var commandHandler = new TeamMemberCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<TeamMember>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(team.DenormalisedNamedDomainModelReference(), newValue.Team);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion
    }
}