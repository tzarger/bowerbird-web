﻿using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using Bowerbird.Core.Config;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using FluentEmail;

namespace Bowerbird.Core.EventHandlers
{
    public class SendWelcomeEmailEventHandler : IEventHandler<DomainModelCreatedEvent<User>>
    {

        #region Members

        private readonly IEmailService _emailService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public SendWelcomeEmailEventHandler(
            IEmailService emailService,
            IConfigService configService)
        {
            Check.RequireNotNull(emailService, "emailService");
            Check.RequireNotNull(configService, "configService");

            _emailService = emailService;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<User> userCreatedEvent)
        {
            Check.RequireNotNull(userCreatedEvent, "userCreatedEvent");

            var message = Email
                .From(_configService.GetEmailAdminAccount(), "Bowerbird")
                .To(userCreatedEvent.DomainModel.Email)
                .Subject("Bowerbird account verification")
                .UsingTemplateFromResource("WelcomeEmail", new { userCreatedEvent.DomainModel.FirstName })
                .Message;

            _emailService.SendMailMessage(message);
        }

        #endregion      

    }
}
