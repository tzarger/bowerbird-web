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

namespace Bowerbird.Core.Commands
{
    public class UserUpdateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public string AvatarId { get; set; }

        public string BackgroundId { get; set; }

        public string DefaultLicence { get; set; }

        public string Timezone { get; set; }

        #endregion

        #region Methods

        #endregion      
     
    }
}