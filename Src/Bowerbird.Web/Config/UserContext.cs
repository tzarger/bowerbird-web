﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Repositories;
using Raven.Client;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;

namespace Bowerbird.Web.Config
{
    public class UserContext : IUserContext
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private PermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public UserContext(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        private PermissionChecker PermissionChecker
        {
            get
            {
                if(_permissionChecker == null)
                {
                    _permissionChecker = new PermissionChecker(_documentSession, GetAuthenticatedUserId());
                }
                return _permissionChecker;
            }
        }

        #endregion

        #region Methods

        public bool IsUserAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public string GetAuthenticatedUserId()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public bool HasEmailCookieValue()
        {
            return GetCookie(Constants.EmailCookieName) != null;
        }

        public string GetEmailCookieValue()
        {
            return GetCookie(Constants.EmailCookieName).Value;
        }

        public void SignUserIn(string email, bool keepUserLoggedIn)
        {
            TimeSpan sessionExpiryDuration;

            if (keepUserLoggedIn)
            {
                // 2 week expiry
                sessionExpiryDuration = new TimeSpan(14, 0, 0, 0);
            }
            else
            {
                // 3 hour expiry
                sessionExpiryDuration = new TimeSpan(3, 0, 0);
            }

            var authTicket = new FormsAuthenticationTicket(_documentSession.LoadUserByEmail(email).Id, keepUserLoggedIn, Convert.ToInt32(sessionExpiryDuration.TotalMinutes)); // Must be less than cookie expiration, whic we have set to 100 years

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            // Add the forms auth session cookie to log user in
            AddCookie(FormsAuthentication.FormsCookieName, encryptedTicket, string.Empty);

            // Add the email into cookie for reference on next login
            AddCookie(Constants.EmailCookieName, email, string.Empty);
        }

        public void SignUserOut()
        {
            FormsAuthentication.SignOut();
        }

        public dynamic GetChannel()
        {
            return Hub.GetClients<ActivityHub>();
        }

        public bool HasGlobalPermission(string permissionId)
        {
            return PermissionChecker.HasGlobalPermission(permissionId);
        }

        public bool HasTeamPermission(string teamId, string permissionId)
        {
            return PermissionChecker.HasTeamPermission(teamId, permissionId);
        }

        public bool HasProjectPermission(string projectId, string permissionId)
        {
            return PermissionChecker.HasProjectPermission(projectId, permissionId);
        }

        public bool HasPermissionToUpdate<T>(string id)
        {
            return PermissionChecker.HasPermissionToUpdate<T>(id);
        }

        public bool HasPermissionToDelete<T>(string id)
        {
            return PermissionChecker.HasPermissionToDelete<T>(id);
        }

        private void AddCookie(string name, string value, string domain)
        {
            HttpCookie cookie = new HttpCookie(name, value)
                                    {
                                        Expires = DateTime.Today.AddYears(100)
                                    };

            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private HttpCookie GetCookie(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }

        #endregion      
      
    }
}
