/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class UserProject : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected UserProject()
            : base()
        {
        }

        public UserProject(
            User createdByUser,
            DateTime createdDateTime)
            : base(
            createdByUser,
            "User Group",
            createdDateTime)
        {
            EventProcessor.Raise(new DomainModelCreatedEvent<UserProject>(this, createdByUser.Id));
        }

        #endregion

        #region Properties

        public override string GroupType
        {
            get { return "userproject"; }
        }
        
        #endregion

        #region Methods

        #endregion
    }
}