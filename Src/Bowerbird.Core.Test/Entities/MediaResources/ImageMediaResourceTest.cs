﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities.MediaResources
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities.MediaResources;

    #endregion

    public class ImageMediaResourceTest
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

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_Constructor_Passing_Width_NotGreaterThanZero_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, 0, FakeValues.Number)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_Constructor_Passing_Height_NotGreaterThanZero_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, FakeValues.Number, 0)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_Constructor_Populates_Property_Values()
        {
            var testMediaResource = new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, FakeValues.Number, FakeValues.Number);

            Assert.AreEqual(testMediaResource.OriginalFileName, FakeValues.Filename);
            Assert.AreEqual(testMediaResource.FileFormat, FakeValues.FileFormat);
            Assert.AreEqual(testMediaResource.Description, FakeValues.Description);
            Assert.AreEqual(testMediaResource.Width, FakeValues.Number);
            Assert.AreEqual(testMediaResource.Height, FakeValues.Number);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_Width_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, FakeValues.Number, FakeValues.Number).Width);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_Height_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, FakeValues.Number, FakeValues.Number).Height);
        }

        #endregion

        #region Method tests

        #endregion
    }
}