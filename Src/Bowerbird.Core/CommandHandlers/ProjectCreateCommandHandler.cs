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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var parentGroup =
                !string.IsNullOrEmpty(command.TeamId)
                    ? (Group)_documentSession.Load<Team>(command.TeamId)
                    : (Group)_documentSession.Load<AppRoot>(Constants.AppRootId);

            var project = new Project(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                command.AvatarId != null ? _documentSession.Load<MediaResource>(command.AvatarId) : null,
                DateTime.UtcNow);

            project.SetAncestry(parentGroup);
            _documentSession.Store(project);

            if (!string.IsNullOrEmpty(command.TeamId))
            {
                parentGroup.AddDescendant(project);
                _documentSession.Store(parentGroup);

                if(parentGroup.Ancestry.Any(x => x.ToLower().Contains("organisations/")))
                {
                    var grandParent = _documentSession.Load<Organisation>(parentGroup.Ancestry.Select(x => x.ToLower().Contains("organisations/")).FirstOrDefault());
                    grandParent.AddDescendant(project);
                    _documentSession.Store(grandParent);
                }
            }

            var projectAdministrator = new Member(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<User>(command.UserId),
                project,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.Equals("roles/projectadministrator") || x.Id.Equals("roles/projectmember"))
                    .ToList()
                );

            _documentSession.Store(projectAdministrator);

            var groupAssociation = new GroupAssociation(
                parentGroup,
                project,
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow
                );

            _documentSession.Store(groupAssociation);
        }

        #endregion
    }
}