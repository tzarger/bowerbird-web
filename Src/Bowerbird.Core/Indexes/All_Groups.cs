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
using Raven.Abstractions.Indexing;
using System.Collections.Generic;

namespace Bowerbird.Core.Indexes
{
    public class All_Groups : AbstractMultiMapIndexCreationTask<All_Groups.Result>
    {
        public class Result
        {
            public string GroupType { get; set; }
            public string GroupId { get; set; }
            public string[] UserIds { get; set; }
            public string ParentGroupId { get; set; }
            public string[] ChildGroupIds { get; set; }
            public string[] AncestorGroupIds { get; set; }
            public string[] DescendantGroupIds { get; set; }
            public string[] GroupRoleIds { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public int UserCount { get; set; }
            public int SightingCount { get; set; }
            public int PostCount { get; set; }
            public int VoteCount { get; set; }
            public string[] LatestObservationIds { get; set; }
            public string[] Categories { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public object[] AllFields { get; set; }
            public string SortName { get; set; }

            public AppRoot AppRoot { get; set; }
            public Organisation Organisation { get; set; }
            public Project Project { get; set; }
            public UserProject UserProject { get; set; }
            public Favourites Favourites { get; set; }
            public IEnumerable<Observation> LatestObservations { get; set; } 

            public Group Group
            {
                get
                {
                    return AppRoot as Group ??
                           UserProject as Group ??
                           Favourites as Group ??
                           Project as Group ??
                           Organisation as Group;
                }
            }
        }


        public All_Groups()  
        {
            AddMap<AppRoot>(
                appRoots => from appRoot in appRoots
                            select new
                            {
                                appRoot.GroupType,
                                GroupId = appRoot.Id,
                                UserIds = new string[] { },
                                ParentGroupId = (string)null,
                                ChildGroupIds = new string[] { },
                                AncestorGroupIds = new string[] { },
                                DescendantGroupIds = new string[] { },
                                GroupRoleIds = new string[] { },
                                appRoot.CreatedDateTime,
                                UserCount = 0,
                                SightingCount = 0,
                                PostCount = 0,
                                VoteCount = 0,
                                LatestObservationIds = new string[] { },
                                Categories = new string[] {},
                                appRoot.Name,
                                Description = (string)null,
                                AllFields = new object[] {},
                                SortName = appRoot.Name
                            });

            AddMap<Organisation>(
                organisations =>
                    from organisation in organisations
                    let parentGroup = organisation.AncestorGroups.FirstOrDefault()
                    select new
                    {
                        organisation.GroupType,
                        GroupId = organisation.Id,
                        UserIds = new string[] { },
                        ParentGroupId = parentGroup.Id,
                        ChildGroupIds = from child in organisation.ChildGroups
                                        select child.Id,
                        AncestorGroupIds = from ancestor in organisation.AncestorGroups
                                           select ancestor.Id,
                        DescendantGroupIds = from descendant in organisation.DescendantGroups
                                             select descendant.Id,
                        GroupRoleIds = new string[] { },
                        organisation.CreatedDateTime,
                        UserCount = 0,
                        SightingCount = 0,
                        PostCount = 0,
                        VoteCount = 0,
                        LatestObservationIds = new string[] { },
                        organisation.Categories,
                        organisation.Name,
                        organisation.Description,
                        AllFields = new[]
                                    {
                                        organisation.Name,
                                        organisation.Description
                                    },
                        SortName = organisation.Name
                    });

            AddMap<Project>(
                projects => from project in projects
                            let parentGroup = project.AncestorGroups.FirstOrDefault()
                            select new
                            {
                                project.GroupType,
                                GroupId = project.Id,
                                UserIds = new string[] { },
                                ParentGroupId = parentGroup.Id,
                                ChildGroupIds = new string[] { },
                                AncestorGroupIds = from ancestor in project.AncestorGroups
                                                   select ancestor.Id,
                                DescendantGroupIds = new string[] { },
                                GroupRoleIds = new string[] { },
                                project.CreatedDateTime,
                                UserCount = 0,
                                SightingCount = 0,
                                PostCount = 0,
                                VoteCount = 0,
                                LatestObservationIds = new string[] { },
                                project.Categories,
                                project.Name,
                                project.Description,
                                AllFields = new[]
                                    {
                                        project.Name,
                                        project.Description
                                    },
                                SortName = project.Name
                            });

            AddMap<UserProject>(
                userProjects => from userProject in userProjects
                                let parentGroup = userProject.AncestorGroups.FirstOrDefault()
                                select new
                                {
                                    userProject.GroupType,
                                    GroupId = userProject.Id,
                                    UserIds = new string[] { },
                                    ParentGroupId = parentGroup.Id,
                                    ChildGroupIds = new string[] { },
                                    AncestorGroupIds = from ancestor in userProject.AncestorGroups
                                                       select ancestor.Id,
                                    DescendantGroupIds = new string[] { },
                                    GroupRoleIds = new string[] { },
                                    userProject.CreatedDateTime,
                                    UserCount = 0,
                                    SightingCount = 0,
                                    PostCount = 0,
                                    VoteCount = 0,
                                    LatestObservationIds = new string[] { },
                                    Categories = new string[] { },
                                    userProject.Name,
                                    Description = (string)null,
                                    AllFields = new object[] { },
                                    SortName = userProject.Name
                                });

            AddMap<Favourites>(
                favouritesGroups => from favourites in favouritesGroups
                                    let parentGroup = favourites.AncestorGroups.FirstOrDefault()
                                    select new
                                        {
                                            favourites.GroupType,
                                            GroupId = favourites.Id,
                                            UserIds = new string[] {},
                                            ParentGroupId = parentGroup.Id,
                                            ChildGroupIds = new string[] {},
                                            AncestorGroupIds = from ancestor in favourites.AncestorGroups
                                                               select ancestor.Id,
                                            DescendantGroupIds = new string[] {},
                                            GroupRoleIds = new string[] {},
                                            favourites.CreatedDateTime,
                                            UserCount = 0,
                                            SightingCount = 0,
                                            PostCount = 0,
                                            VoteCount = 0,
                                            LatestObservationIds = new string[] { },
                                            Categories = new string[] { },
                                            favourites.Name,
                                            Description = (string)null,
                                            AllFields = new object[] { },
                                            SortName = favourites.Name
                                        });

            // Memberships of groups
            AddMap<User>(
                users => from user in users
                         from member in user.Memberships
                         select new
                         {
                             member.Group.GroupType,
                             GroupId = member.Group.Id,
                             UserIds = new [] {user.Id },
                             ParentGroupId = (string)null,
                             ChildGroupIds = new string[] { },
                             AncestorGroupIds = new string[] { },
                             DescendantGroupIds = new string[] { },
                             GroupRoleIds = member.Roles.Select(x => x.Id),
                             CreatedDateTime = (object)null,
                             UserCount = 1,
                             SightingCount = 0,
                             PostCount = 0,
                             VoteCount = 0,
                             LatestObservationIds = new string[] { },
                             Categories = new string[] { },
                             Name = (string)null,
                             Description = (string)null,
                             AllFields = new object[] { },
                             SortName = (string)null
                         });

            // Count of observation sightings in groups
            AddMap<Observation>(
                observations => from observation in observations
                                from sightingGroup in observation.Groups
                                select new
                                    {
                                        sightingGroup.Group.GroupType,
                                        GroupId = sightingGroup.Group.Id,
                                        UserIds = new string[] {},
                                        ParentGroupId = (string) null,
                                        ChildGroupIds = new string[] {},
                                        AncestorGroupIds = new string[] {},
                                        DescendantGroupIds = new string[] {},
                                        GroupRoleIds = new string[] {},
                                        CreatedDateTime = (object) null,
                                        UserCount = 0,
                                        SightingCount = 1,
                                        PostCount = 0,
                                        VoteCount = 0,
                                        LatestObservationIds = new [] { observation.Id },
                                        Categories = new string[] { },
                                        Name = (string)null,
                                        Description = (string)null,
                                        AllFields = new object[] { },
                                        SortName = (string)null
                                    });

            // Count of record sightings in groups
            AddMap<Record>(
                records => from record in records
                           from sightingGroup in record.Groups
                           select new
                               {
                                   sightingGroup.Group.GroupType,
                                   GroupId = sightingGroup.Group.Id,
                                   UserIds = new string[] {},
                                   ParentGroupId = (string) null,
                                   ChildGroupIds = new string[] {},
                                   AncestorGroupIds = new string[] {},
                                   DescendantGroupIds = new string[] {},
                                   GroupRoleIds = new string[] {},
                                   CreatedDateTime = (object) null,
                                   UserCount = 0,
                                   SightingCount = 1,
                                   PostCount = 0,
                                   VoteCount = 0,
                                   LatestObservationIds = new string[] { },
                                   Categories = new string[] { },
                                   Name = (string)null,
                                   Description = (string)null,
                                   AllFields = new object[] { },
                                   SortName = (string)null
                               });

            // Count of posts in groups
            AddMap<Post>(
                posts => from post in posts
                         select new
                         {
                             post.Group.GroupType,
                             GroupId = post.Group.Id,
                             UserIds = new string[] { },
                             ParentGroupId = (string)null,
                             ChildGroupIds = new string[] { },
                             AncestorGroupIds = new string[] { },
                             DescendantGroupIds = new string[] { },
                             GroupRoleIds = new string[] { },
                             CreatedDateTime = (object)null,
                             UserCount = 0,
                             SightingCount = 0,
                             PostCount = 1,
                             VoteCount = 0,
                             LatestObservationIds = new string[] { },
                             Categories = new string[] { },
                             Name = (string)null,
                             Description = (string)null,
                             AllFields = new object[] { },
                             SortName = (string)null
                         });

            Reduce = results => from result in results
                                group result by result.GroupId
                                    into g
                                    select new
                                    {
                                        GroupType = g.Select(x => x.GroupType).FirstOrDefault(),
                                        GroupId = g.Key,
                                        UserIds = g.SelectMany(x => x.UserIds),
                                        ParentGroupId = g.Select(x => x.ParentGroupId).Where(x => x != null).FirstOrDefault(),
                                        ChildGroupIds = g.SelectMany(x => x.ChildGroupIds),
                                        AncestorGroupIds = g.SelectMany(x => x.AncestorGroupIds),
                                        DescendantGroupIds = g.SelectMany(x => x.DescendantGroupIds),
                                        GroupRoleIds = g.SelectMany(x => x.GroupRoleIds),
                                        CreatedDateTime = g.Select(x => x.CreatedDateTime).Where(x => x != null).FirstOrDefault(),
                                        UserCount = g.Sum(x => x.UserCount),
                                        SightingCount = g.Sum(x => x.SightingCount),
                                        PostCount = g.Sum(x => x.PostCount),
                                        VoteCount = g.Sum(x => x.VoteCount),
                                        LatestObservationIds = g.SelectMany(x => x.LatestObservationIds).OrderByDescending(x => x).Take(10), // Just ordering by string, lexicographically
                                        Categories = g.SelectMany(x => x.Categories),
                                        Name = g.Select(x => x.Name).Where(x => x != null).FirstOrDefault(),
                                        Description = g.Select(x => x.Description).Where(x => x != null).FirstOrDefault(),
                                        AllFields = g.SelectMany(x => x.AllFields),
                                        SortName = g.Select(x => x.SortName).Where(x => x != null).FirstOrDefault(),
                                    };

            TransformResults = (database, results) =>
                from result in results
                select new
                {
                    result.GroupType,
                    result.GroupId,
                    UserIds = result.UserIds ?? new string[]{},
                    result.ParentGroupId,
                    ChildGroupIds = result.ChildGroupIds ?? new string[] { },
                    AncestorGroupIds = result.AncestorGroupIds ?? new string[]{},
                    DescendantGroupIds = result.DescendantGroupIds ?? new string[]{},
                    GroupRoleIds = result.GroupRoleIds ?? new string[] { },
                    result.UserCount,
                    result.SightingCount,
                    result.PostCount,
                    result.VoteCount,
                    AppRoot = result.GroupType == "approot" ? database.Load<AppRoot>(result.GroupId) : null,
                    Organisation = result.GroupType == "organisation" ? database.Load<Organisation>(result.GroupId) : null,
                    Project = result.GroupType == "project" ? database.Load<Project>(result.GroupId) : null,
                    UserProject = result.GroupType == "userproject" ? database.Load<UserProject>(result.GroupId) : null,
                    Favourites = result.GroupType == "favourites" ? database.Load<Favourites>(result.GroupId) : null,
                    LatestObservations = database.Load<Observation>(result.LatestObservationIds)
                };

            Store(x => x.GroupType, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.UserIds, FieldStorage.Yes);
            Store(x => x.ParentGroupId, FieldStorage.Yes);
            Store(x => x.ChildGroupIds, FieldStorage.Yes);
            Store(x => x.AncestorGroupIds, FieldStorage.Yes);
            Store(x => x.DescendantGroupIds, FieldStorage.Yes);
            Store(x => x.GroupRoleIds, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.CreatedDateTime, FieldStorage.Yes);
            Store(x => x.UserCount, FieldStorage.Yes);
            Store(x => x.SightingCount, FieldStorage.Yes);
            Store(x => x.PostCount, FieldStorage.Yes);
            Store(x => x.VoteCount, FieldStorage.Yes);
            Store(x => x.LatestObservationIds, FieldStorage.Yes);
            Store(x => x.Categories, FieldStorage.Yes);
            Store(x => x.SortName, FieldStorage.Yes);

            Index(x => x.Name, FieldIndexing.Analyzed);
            Index(x => x.Description, FieldIndexing.Analyzed);
            Index(x => x.AllFields, FieldIndexing.Analyzed);

            Sort(x => x.UserCount, SortOptions.Int);
        }
    }
}