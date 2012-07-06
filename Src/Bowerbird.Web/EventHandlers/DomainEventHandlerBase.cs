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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using SignalR.Hubs;

namespace Bowerbird.Web.EventHandlers
{
    public abstract class DomainEventHandlerBase
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected Activity MakeActivity<T>(T domainEvent, string type, string description, IEnumerable<dynamic> groups) where T : IDomainEvent
        {
            var now = DateTime.UtcNow;

            return new Activity(
                type,
                DateTime.UtcNow,
                description,
                new 
                {
                    domainEvent.User.Id,
                    domainEvent.User.FirstName,
                    domainEvent.User.LastName,
                    domainEvent.User.Avatar
                },
                groups);
        }

        protected AsynchActivity MakeAsynchActivity<T>(T domainEvent, string type, object model, string clientId) where T : IDomainEvent
        {
            var now = DateTime.UtcNow;

            return new AsynchActivity(
                type,
                DateTime.UtcNow,
                model,
                clientId);
        }

        #endregion
    }
}
