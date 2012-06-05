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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityObservationAdded : DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Observation>>, 
        IEventHandler<DomainModelCreatedEvent<ObservationGroup>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public ActivityObservationAdded(
            IDocumentSession documentSession,
            INotificationService notificationService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationService, "notificationService");

            _documentSession = documentSession;
            _notificationService = notificationService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} add an observation", domainEvent.User.GetName()), 
                domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.ObservationAdded = new
            {
                Observation = domainEvent.DomainModel
            };

            _documentSession.Store(activity);
            _notificationService.SendActivity(activity);
        }

        public void Handle(DomainModelCreatedEvent<ObservationGroup> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} add an observation", domainEvent.User.GetName()), 
                new[] { domainEvent.DomainModel.Group });

            activity.ObservationAdded = new
            {
                Observation = domainEvent.Sender
            };

            _documentSession.Store(activity);
            _notificationService.SendActivity(activity);
        }

        #endregion      
    }
}