﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Passes a model directly back to a particular client. This is used when media resources
    /// are uploaded in a disconnected, asynch process. Allows model to upload independently of
    /// web request and return back to same client.
    /// </summary>
    public class MediaResourceUploaded : IEventHandler<DomainModelCreatedEvent<MediaResource>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public MediaResourceUploaded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<MediaResource> domainEvent)
        {
            var user = _documentSession.Load<User>(domainEvent.User.Id);

            var mediaResource = new
            {
                domainEvent.DomainModel.Id,
                domainEvent.DomainModel.Metadata,
                domainEvent.DomainModel.MediaType,
                domainEvent.DomainModel.Key,
                domainEvent.DomainModel.Files
            };

            _backChannelService.SendUploadedMediaResourceToUserChannel(domainEvent.User.Id, mediaResource);
        }

        #endregion      
    }
}