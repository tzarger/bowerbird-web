/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.ViewModels;

namespace Bowerbird.Core.Queries
{
    public interface IPostViewModelQuery : IQuery
    {
        object BuildCreatePost(string groupId);

        object BuildPost(string id);

        object BuildGroupPostList(string groupId, PostsQueryInput postsQueryInput);

        object BuildHomePostList(string userId, PostsQueryInput postsQueryInput);
    }
}