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

namespace Bowerbird.Core.EventHandlers
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

        protected Activity MakeActivity<T>(
            T domainEvent, 
            string type, 
            DateTime created,
            string description, 
            IEnumerable<dynamic> groups,
            string contributionId = (string)null,
            string subContributionId = (string)null
            ) 
            where T : IDomainEvent
        {
            return new Activity(
                type,
                created,
                description,
                new 
                {
                    domainEvent.User.Id,
                    domainEvent.User.Name,
                    domainEvent.User.Avatar
                },
                groups,
                contributionId,
                subContributionId);
        }

        #endregion
    }
}
