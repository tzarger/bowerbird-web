﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities.DenormalisedReferences
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Entities.DenormalisedReferences;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class DenormalisedNamedEntityReferenceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static DenormalisedNamedEntityReference<T> TestDenormalise<T>(T t) where T : INamedEntity
        {
            DenormalisedNamedEntityReference<T> denormalisedObservationReference = t;

            return denormalisedObservationReference;
        }
        
        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedEntityReference_Implicit_Operator_Passing_Project_Returns_DenormalisedObservationReference_With_Populated_Properties()
        {
            var normalisedProject = FakeObjects.TestProject();

            DenormalisedNamedEntityReference<Project> denormalisedProject = TestDenormalise(normalisedProject);

            Assert.AreEqual(denormalisedProject.Id, normalisedProject.Id);
            Assert.AreEqual(denormalisedProject.Name, normalisedProject.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedEntityReference_Implicit_Operator_Passing_Team_Returns_DenormalisedObservationReference_With_Populated_Properties()
        {
            var normalisedTeam = FakeObjects.TestTeam();

            DenormalisedNamedEntityReference<Team> denormalisedTeam = TestDenormalise(normalisedTeam);

            Assert.AreEqual(denormalisedTeam.Id, normalisedTeam.Id);
            Assert.AreEqual(denormalisedTeam.Name, normalisedTeam.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedNamedEntityReference_Implicit_Operator_Passing_Null_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => TestDenormalise<Project>(null)));
        }

        #endregion
    }
}