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
    public class ActivityCommentAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Comment>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityCommentAdded(
            IDocumentSession documentSession,
           IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Comment> domainEvent)
        {
            if(domainEvent.Sender is Observation)
            {
                var observation = domainEvent.Sender as Observation;

                dynamic activity = MakeActivity(
                    domainEvent, 
                    "observationcommentadded", 
                    domainEvent.DomainModel.CreatedOn,
                    string.Format("{0} added a comment to observation {1}", 
                    domainEvent.User.GetName(), 
                    ((Observation)observation).Title), 
                    ((Observation)observation).Groups.Select(x => x.Group),
                    observation.Id);

                activity.ObservationCommentAdded = new
                {
                    Comment = domainEvent.DomainModel
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }

            if(domainEvent.Sender is Post)
            {
                var post = domainEvent.Sender as Post;
                var group = _documentSession.Load<dynamic>(((Post) post).Group.Id);

                dynamic activity = MakeActivity(
                    domainEvent,
                    "postcommentadded",
                    domainEvent.DomainModel.CreatedOn,
                    string.Format("{0} added a comment to news item {1}",
                    domainEvent.User.GetName(),
                    ((Post)post).Subject),
                    new []{group},
                    post.Id);

                activity.PostCommentAdded = new
                {
                    Comment = domainEvent.DomainModel
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }
        }

        #endregion     
    }
}