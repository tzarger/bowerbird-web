/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using System.Collections.Generic;

namespace Bowerbird.Web.Factories
{
    public interface IUserViewFactory
    {
        object Make(string userId);

        object Make(User user);

        object Make(User user, IEnumerable<Member> memberships);
    }
}