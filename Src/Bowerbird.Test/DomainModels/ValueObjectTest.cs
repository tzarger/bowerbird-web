﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.DomainModels
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture, Ignore]
    public class ValueObjectTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ValueObject_Equality_Operator_Comparing_Two_Equal_Objects_Returns_True()
        {
            var leftValueObject = new ProxyObjects.ProxyValueObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());
            var rightValueObject = new ProxyObjects.ProxyValueObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());

            //Assert.IsTrue(leftValueObject.HasSameObjectSignatureAs(rightValueObject));
            var leftHashCode = leftValueObject.GetHashCode();
            var rightHashCode = rightValueObject.GetHashCode();

            Assert.AreNotEqual(leftHashCode, rightHashCode);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void BaseObject_Equality_Operator_Comparing_Two_Equal_Objects_Returns_True()
        {
            var leftValueObject = new ProxyObjects.ProxyBaseObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());
            var rightValueObject = new ProxyObjects.ProxyBaseObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());

            Assert.IsTrue(leftValueObject.Equals(rightValueObject));
            //var leftHashCode = leftValueObject.GetHashCode();
            //var rightHashCode = rightValueObject.GetHashCode();

            //Assert.AreNotEqual(leftHashCode, rightHashCode);
        }

        #endregion 
    }
}