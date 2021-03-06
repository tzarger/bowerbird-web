﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

//namespace Bowerbird.Test.EventHandlers
//{
//    #region Namespaces

//    using System.Collections.Generic;

//    using NUnit.Framework;
//    using Moq;

//    using Bowerbird.Core.DesignByContract;
//    using Bowerbird.Core.DomainModels;
//    using Bowerbird.Web.Config;
//    using Bowerbird.Web.EventHandlers;
//    using Bowerbird.Test.Utils;

//    #endregion

//    [TestFixture] public class NotifyActivityObservationCreatedEventHandlerTest
//    {
//        #region Test Infrastructure

//        private Mock<IUserContext> _mockUserContext;

//        [SetUp] public void TestInitialize() { _mockUserContext = new Mock<IUserContext>(); }

//        [TearDown] public void TestCleanup() { }

//        #endregion

//        #region Test Helpers

//        private sealed class ProxyNotifyActivityObservationCreatedEventHandler : NotifyActivityObservationCreatedEventHandler
//        {
//            public ProxyNotifyActivityObservationCreatedEventHandler(IUserContext userContext) : base(userContext) { }

//            public new void Notify(string type, User user, object data)
//            {
//                base.Notify(type, user, data);
//            }
//        }

//        /// <summary>
//        /// Id: "abc"
//        /// Password: "password"
//        /// Email: "padil@padil.gov.au"
//        /// FirstName: "first name"
//        /// LastName: "last name"
//        /// Description: "description"
//        /// Roles: "Member"
//        /// </summary>
//        /// <returns></returns>
//        private static User TestUser()
//        {
//            return new User(
//                FakeValues.Password,
//                FakeValues.Email,
//                FakeValues.FirstName,
//                FakeValues.LastName,
//                TestRoles()
//            )
//            .UpdateLastLoggedIn()
//            .IncrementFlaggedItemsOwned()
//            .IncrementFlagsRaised();
//        }

//        private static IEnumerable<Role> TestRoles()
//        {
//            return new List<Role>()
//            {
//                new Role
//                (
//                    "Member",
//                    "Member role",
//                    "Member description",
//                    TestPermissions()
//                )
//            };
//        }

//        private static IEnumerable<Permission> TestPermissions()
//        {
//            return new List<Permission>
//            {
//                new Permission("Read", "Read permission", "Read description"),
//                new Permission("Write", "Write permission", "Write description")
//            };

//        }

//        #endregion

//        #region Constructor tests

//        [Test, Category(TestCategory.Unit)] public void NotifyActivityObservationCreatedEventHandler_Constructor_Passing_Null_UserContext_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new NotifyActivityObservationCreatedEventHandler(null)));
//        }

//        #endregion

//        #region Property tests

//        #endregion

//        #region Method tests

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void NotifyActivityObservationCreatedEventHandler_Notify_Passing_Empty_Type_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(() => 
//                    new ProxyNotifyActivityObservationCreatedEventHandler(_mockUserContext.Object).Notify(string.Empty, TestUser(), new object())));
//        }

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void NotifyActivityObservationCreatedEventHandler_Notify_Passing_Null_User_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(() => 
//                    new ProxyNotifyActivityObservationCreatedEventHandler(_mockUserContext.Object).Notify(FakeValues.ActivityType, null, new object())));
//        }

//        [Test]
//        [Category(TestCategory.Unit)] public void NotifyActivityObservationCreatedEventHandler_Notify_Passing_Null_Data_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(() =>
//                    new ProxyNotifyActivityObservationCreatedEventHandler(_mockUserContext.Object).Notify(FakeValues.ActivityType, TestUser(), null)));
//        }

//        // TODO: This test is unfinished - do we need further mocking of dynamic clients object?
//        //[Test,Ignore]
//        //[Category(TestCategory.Unit)]
//        //public void NotifyActivityObservationCreatedEventHandler_Notify_Calls_UserContext_GetChannel()
//        //{
//        //    var clients = new
//        //    {
//        //        activityOccurred = "string"
//        //    };

//        //    _mockUserContext.Setup(x => x.GetChannel()).Returns(clients).Verifiable();

//        //    new ProxyNotifyActivityObservationCreatedEventHandler(_mockUserContext.Object).Notify(FakeValues.ActivityType, TestUser(), new object());

//        //    _mockUserContext.Verify();
//        //}

//        #endregion					
//    }
//}