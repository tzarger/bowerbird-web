﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Core.Test.EventHandlers
{
    #region Namespaces

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.EventHandlers;
    using Bowerbird.Test.Utils;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class SendWelcomeEmailEventHandlerTest
    {
        #region Infrastructure

        [SetUp]
        public void TestInitialize()
        {

        }

        [TearDown]
        public void TestCleanup()
        {

        }

        #endregion

        #region Helpers


        #endregion

        #region Constructor tests


        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test, Category(TestCategories.Unit)]
        public void SendWelcomeEmailEventHandler_Handle_Passing_Null_UserCreatedEvent_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>new SendWelcomeEmailEventHandler().Handle(null)));
        }

        [Test, Ignore]
        public void SendWelcomeEmailEventHandler_Handle_Passing_UserCreatedEvent_DoesStuffYetDefined()
        {
            
        }

        #endregion					
    }
}