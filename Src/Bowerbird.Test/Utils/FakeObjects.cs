﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.Utils
{
    #region Namespaces

    using System.Collections.Generic;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class FakeObjects
    {
        public static User TestUser()
        {
            return new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                TestRoles()
                )
                .UpdateLastLoggedIn()
                .IncrementFlaggedItemsOwned()
                .IncrementFlagsRaised();
        }

        public static User TestUserWithId()
        {
            User user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                TestRoles()
                )
                .UpdateLastLoggedIn()
                .IncrementFlaggedItemsOwned()
                .IncrementFlagsRaised();

            ((IAssignableId)user).SetIdTo("users", FakeValues.UserId);

            return user;
        }

        public static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        public static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };
        }
        
        public static Project TestProject()
        {
            return new Project(TestUser(), FakeValues.Name, FakeValues.Description);
        }

        public static Project TestProjectWithId()
        {
            Project project =  new Project(TestUser(), FakeValues.Name, FakeValues.Description);

            ((IAssignableId)project).SetIdTo("projects", FakeValues.KeyString);

            return project;
        }

        public static Team TestTeam()
        {
            return new Team(TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website);
        }

        public static Post TestPost()
        {
            return new ProxyObjects.ProxyPost(TestUser(), FakeValues.CreatedDateTime, FakeValues.Subject, FakeValues.Message, new List<MediaResource>());
        }

        public static ProjectMember TestProjectMember()
        {
            return new ProjectMember(
                FakeObjects.TestUser(),
                FakeObjects.TestProjectWithId(),
                FakeObjects.TestUserWithId(),
                FakeObjects.TestRoles());
        }

        public static Observation TestObservationWithId()
        {
            var observation = new Observation(
                TestUserWithId(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new List<MediaResource>()
                );

            ((IAssignableId)observation).SetIdTo("observationcomments", FakeValues.KeyString);

            return observation;
        }

        public static Observation TestObservationWithId(string id)
        {
            var observation = new Observation(
                TestUserWithId(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new List<MediaResource>()
                );

            ((IAssignableId)observation).SetIdTo("observationcomments", id);

            return observation;
        }
    }
}