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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectDeleteCommandHandler : ICommandHandler<ProjectDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods
        
        public void Handle(ProjectDeleteCommand command)
        {
            //Check.RequireNotNull(command, "command");

            //var project = _documentSession.Load<Project>(command.Id);

            //var team = _documentSession
            //    .Query<Team>()
            //    .Where(x => x.DescendantGroups.Any(y => y.Id.ToLower() == project.Id.ToLower()))
            //    .FirstOrDefault();

            //if(team != null)
            //{
            //    team.RemoveDescendantGroup(project);

            //    var organisation = _documentSession
            //        .Query<Organisation>()
            //        .Where(x => x.DescendantGroups.Any(y => y.Id.ToLower() == project.Id.ToLower()))
            //        .FirstOrDefault();

            //    if(organisation != null)
            //    {
            //        organisation.RemoveDescendantGroup(project);
            //    }
            //}

            //_documentSession.Delete(project);
        }

        #endregion				
    }
}