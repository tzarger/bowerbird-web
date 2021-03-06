﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Net;
using System.Net.Mail;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Config;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Services
{
    public class EmailService : IEmailService
    {
            
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IConfigSettings _configSettings;
        private readonly ISystemStateManager _systemStateManager;

        #endregion

        #region Constructors

        public EmailService(
            IDocumentSession documentSession,
            IConfigSettings configService,
            ISystemStateManager systemStateManager)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(systemStateManager, "systemStateManager");

            _documentSession = documentSession;
            _configSettings = configService;
            _systemStateManager = systemStateManager;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendMailMessage(MailMessage mailMessage)
        {
            var smtpClient = new SmtpClient();

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            if (appRoot != null && appRoot.EmailServiceStatus)
            {
                smtpClient.SendAsync(mailMessage, null);
            }
        }

        #endregion
      
    }
}
