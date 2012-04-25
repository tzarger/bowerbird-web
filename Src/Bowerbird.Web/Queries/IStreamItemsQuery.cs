/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Queries
{
    public interface IStreamItemsQuery
    {
        PagedList<StreamItem> GetStreamItems(StreamItemListInput listInput, StreamSortInput sortInput);
    }
}