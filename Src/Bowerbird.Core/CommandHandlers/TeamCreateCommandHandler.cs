/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamCreateCommandHandler : ICommandHandler<TeamCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var team = new Team(
                _documentSession.Load<User>(command.UserId)
                , command.Name
                , command.Description
                , command.Website
                , command.AvatarId != null ? _documentSession.Load<MediaResource>(command.AvatarId) : null
                );

            _documentSession.Store(team);
        }

        #endregion

    }
}