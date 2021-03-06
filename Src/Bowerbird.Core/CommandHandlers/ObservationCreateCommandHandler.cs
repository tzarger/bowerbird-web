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
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationCreateCommandHandler : ICommandHandler<ObservationCreateCommand, Observation>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods 

        public Observation HandleReturn(ObservationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            List<MediaResource> mediaResources;
            var user = _documentSession.Load<User>(command.UserId);
            var userProject = _documentSession.Load<UserProject>(user.UserProject.Id);

            // the presumption here is that there is either media referenced by mediaresourceid or key.
            bool mediaByResourceId = command.Media.Any(x => x.MediaResourceId != null);

            if (mediaByResourceId)
            {
                var mediaResourceIds = command.Media.Select(x => x.MediaResourceId).ToList();
                
                mediaResources = _documentSession.Load<MediaResource>(mediaResourceIds).ToList();
            }
            else
            {
                var mediaResourceIds = command.Media.Select(x => x.Key).ToList();

                mediaResources = _documentSession
                    .Query<MediaResource>()
                    .Where(x => x.Key.In(mediaResourceIds))
                    .ToList();
            }

            IEnumerable<Project> projects = new List<Project>();

            if (command.Projects != null && command.Projects.Any())
            {
                projects = _documentSession.Load<Project>(command.Projects);
            }

            // Ensure at least one media set as primary
            if(command.Media.All(x => !x.IsPrimaryMedia))
            {
                command.Media.First().IsPrimaryMedia = true;
            }

            var observation = new Observation(
                command.Key,
                user,
                command.Title,
                DateTime.UtcNow,
                command.ObservedOn,
                command.Latitude,
                command.Longitude,
                command.Address,
                command.AnonymiseLocation,
                command.Category,
                userProject,
                projects,
                command.Media.Select(x => // command media may either have ids or keys depending on which client...
                new Tuple<MediaResource, string, string, bool>(
                    x.MediaResourceId != null ? mediaResources.Single(y => y.Id == x.MediaResourceId) : mediaResources.Single(y => y.Key == x.Key),
                    x.Description,
                    x.Licence,
                    x.IsPrimaryMedia)));

            _documentSession.Store(observation);
            _documentSession.SaveChanges();

            return observation;
        }

        #endregion      
      
    }
}
