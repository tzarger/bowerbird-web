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

using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class MediaResourceCreateFailedEvent : DomainEventBase
    {

        #region Members

        #endregion

        #region Constructors

        public MediaResourceCreateFailedEvent(
            User user,
            string key,
            string reason,
            DomainModel sender)
            : base(
            user,
            sender)
        {
            Key = key;
            Reason = reason;
        }

        #endregion

        #region Properties

        public string Key { get; private set; }

        public string Reason { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}