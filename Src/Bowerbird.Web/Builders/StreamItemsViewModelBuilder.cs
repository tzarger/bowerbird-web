﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Config;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class StreamItemsViewModelBuilder : IStreamItemsViewModelBuilder
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IObservationViewFactory _observationViewFactory;

        #endregion

        #region Constructors

        public StreamItemsViewModelBuilder(
            IUserContext userContext,
            IDocumentSession documentSession,
            IStreamItemFactory streamItemFactory,
            IObservationViewFactory observationViewFactory)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _streamItemFactory = streamItemFactory;
            _observationViewFactory = observationViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public PagedList<object> BuildUserStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x => new { GroupId = x.Group.Id })
                .ToList();

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults);
        }

        public PagedList<object> BuildHomeStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x => new { GroupId = x.Group.Id })
                .ToList();

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults);
        }

        public PagedList<object> BuildGroupStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == pagingInput.Id)
                .ToList()
                .Select(x => new { GroupId = x.ChildGroupId })
                .ToList();

            groups.Add(new { GroupId = pagingInput.Id });

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    item = _observationViewFactory.Make(groupContributionResult.Observation);
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                groups,
                "observation",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }

        //// Get all stream items for all groups that a particular user is a member of
        //private PagedList<StreamItem> MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        //{
        //    RavenQueryStatistics stats;

        //    var groupMemberships = _documentSession
        //        .Query<Member>()
        //        .Include(x => x.User.Id)
        //        .Where(x => x.User.Id == listInput.UserId);
        //        //.ToList();

        //    var groupContributions = GetContributionsForGroups(groupMemberships)
        //        .AsProjection<All_Contributions.Result>()
        //        .Include(x => x.ContributionId)
        //        .Where(x => x.UserId == listInput.UserId)
        //        .Statistics(out stats)
        //        .Skip(listInput.Page)
        //        .Take(listInput.PageSize)
        //        .ToList();

        //    //SortResults(groupContributions, sortInput);

        //    //RavenQueryStatistics stats = ProjectGroupContributions(listInput, groupContributions);

        //    return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
        //}

        //// Get all items that match the query of a users' watchlist
        //private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        //{
        //    var memberWatchlists = _documentSession.Load<Watchlist>(listInput.WatchlistId);

        //    // TODO: Create an index to query for watchlist 

        //    //var groupContributions = _documentSession
        //    //    .Query<GroupContributionResults, All_GroupContributionItems>()
        //    //    .Include(x => x.ContributionId)
        //    //    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

        //    //SortResults(groupContributions, sortInput);

        //    //RavenQueryStatistics stats;

        //    //groupContributions
        //    //    .OrderByDescending(x => x.GroupCreatedDateTime)
        //    //    .AsProjection<GroupContributionResults>()
        //    //    .Statistics(out stats)
        //    //    .Skip(listInput.Page)
        //    //    .Take(listInput.PageSize)
        //    //    .ToList();

        //    //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);

        //    return new StreamItemList();
        //}

        #endregion
    }
}