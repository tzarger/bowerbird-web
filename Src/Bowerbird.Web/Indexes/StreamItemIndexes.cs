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
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;

namespace Bowerbird.Web.Indexes
{
    public class StreamItem_WithParentIdAndUserIdAndCreatedDateTimeAndType : AbstractIndexCreationTask<StreamItem>
    {
        public StreamItem_WithParentIdAndUserIdAndCreatedDateTimeAndType()
        {
            Map = streamItems => from streamItem in streamItems

                                 select new
                                            {
                                                streamItem.ParentId, 
                                                UserId = streamItem.User.Id, 
                                                streamItem.CreatedDateTime, 
                                                streamItem.Type, 
                                                streamItem.ItemId
                                            };
        }
    }

    //public class StreamItem_DefaultForUser : AbstractIndexCreationTask<StreamItem>
    //{
    //    public StreamItem_WithStuff()
    //    {
    //        Map = streamItems => from streamItem in streamItems

    //                             select new
    //                             {
    //                                 streamItem.ParentId,
    //                                 UserId = streamItem.User.Id,
    //                                 streamItem.CreatedDateTime,
    //                                 streamItem.Type,
    //                                 streamItem.ItemId
    //                             };

    //        TransformResults = (database, results) =>
    //                           from result in results
    //                           let observation = database.Load<Observation>(result.ItemId)
    //                           let post = database.Load<Post>(result.ItemId)
    //                           let observationNote = database.Load<ObservationNote>(result.ItemId)
    //                           select new
    //                            {
    //                                Item = post ?? observation ?? observationNote,

    //                            };
    //    }
    //}

    public class ResultModel
    {
        public string ItemId { get; set; }
        public DateTime SubmittedOn { get; set; }
        public object GroupItem { get; set; }
        public string[] ProjectIds { get; set; }
        public string[] TeamIds { get; set; }
        public string[] OrganisationIds { get; set; }
    }

    //public class StreamItem_By : AbstractMultiMapIndexCreationTask<ResultModel>
    //{
    //    public StreamItem_By()
    //    {
    //        AddMap<Observation>(observations => from observation in observations
    //                                            select new
    //                                                       {
    //                                                           ItemId = observation.Id,
    //                                                           SubmittedOn = observation.SubmittedOn,
    //                                                           GroupItem = observation,
    //                                                           ProjectIds = observation.Projects.Select(x => x.Id),
    //                                                           TeamIds = new string []{},
    //                                                           OrganisationIds = new string[] { }
    //                                                       });

    //        AddMap<Post>(posts => from post in posts
    //                              select new
    //                                         {
    //                                             ItemId = post.Id,
    //                                             SubmittedOn = post.PostedOn,
    //                                             GroupItem = post,
    //                                             ProjectIds = post is ProjectPost ? new [] { ((ProjectPost)post).Project.Id } : new string[]{},
    //                                             TeamIds = post is TeamPost ? new[] { ((TeamPost)post).Team.Id } : new string[] { },
    //                                             OrganisationIds = post is OrganisationPost ? new [] { ((OrganisationPost)post).Organisation.Id } : new string[]{}
    //                                         });

    //        AddMap<ObservationNote>(observationNotes => from observationNote in observationNotes
    //                                                    select new
    //                                                               {
    //                                                                   ItemId = observationNote.Id,
    //                                                                   SubmittedOn = observationNote.SubmittedOn,
    //                                                                   GroupItem = observationNote,
    //                                                                   ProjectIds = new string[] {},
    //                                                                   TeamIds = new string[] { },
    //                                                                   OrganisationIds = new string[] { }
    //                                                               });

    //        TransformResults = (database, groupItems) => from groupItem in groupItems
    //                                                     let projects = database.Load<Project>(groupItem.ProjectIds)
    //                                                     let teams = database.Load<Team>(groupItem.TeamIds)
    //                                                     let organisations = database.Load<Organisation>(groupItem.OrganisationIds)
    //                                                     select new
    //                                                                {
    //                                                                    ProjectIds = projects.Select(x => x.Id) 
    //                                                                };

    //        //TransformResults = (database, groupItems) => from groupItem in groupItems
    //        //                                             let projectObservations = database.Load<ProjectObservation>(groupItem.ParentId)
    //        //                                             select new
    //        //                                                        {
    //        //                                                            Frank = groupItem.ItemId
    //        //                                                        };
    //    }
    //}
}