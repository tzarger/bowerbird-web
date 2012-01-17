/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels
{
    public class ProjectIndexInput
    {
        #region Members

        #endregion

        #region Constructors

        public ProjectIndexInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string ProjectId { get; set; }

        public string UserId { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        #endregion
    }
}