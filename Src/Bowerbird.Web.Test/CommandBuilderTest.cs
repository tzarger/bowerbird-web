﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test
{
    #region Namespaces

    using System;

    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Moq;

    using Bowerbird.Web;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Commands;
    using Bowerbird.Web.CommandFactories;
    using Bowerbird.Web.ViewModels;

    #endregion

    [TestFixture] 
    public class CommandBuilderTest
    {
        #region Test Infrastructure

        private Mock<IServiceLocator> _mockServiceLocator;
        private CommandBuilder _commandBuilder;

        [SetUp] 
        public void TestInitialize()
        {
            BootstrapperHelper.Startup();

            _mockServiceLocator = new Mock<IServiceLocator>();

            _commandBuilder = new CommandBuilder(_mockServiceLocator.Object);
        }

        [TearDown] 
        public void TestCleanup()
        {
            BootstrapperHelper.Shutdown();
        }

        #endregion

        #region Test Helpers

        private bool TestCommandAction()
        {
            return false; 
        }

        private ObservationCreateInput TestObservationCreateInput()
        {
            return new ObservationCreateInput()
            {
                Address = FakeValues.Address,
                Description = FakeValues.Description,
                IsIdentificationRequired = FakeValues.IsTrue,
                Latitude = FakeValues.Latitude,
                Longitude = FakeValues.Longitude,
                ObservationCategory = FakeValues.Category,
                ObservedOn = FakeValues.CreatedDateTime,
                Title = FakeValues.Title,
                Username = FakeValues.UserName
            };
        }

        #endregion

        #region Constructor tests

        [Test, Category(TestCategories.Unit)] 
        public void CommandBuilder_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new CommandBuilder(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategories.Unit)] 
        public void CommandBuilder_Build_Passing_Null_Input_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _commandBuilder.Build<object, object>(null, x => TestCommandAction())));
        }

        [Test, Category(TestCategories.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Having_ServiceLocator_Not_Find_Instance_Throws_Exception()
        {
            var input = TestObservationCreateInput();

            ICommandFactory<ObservationCreateInput, ObservationCreateCommand> commandFactory = null;

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(commandFactory);

            Assert.IsTrue(BowerbirdThrows.Exception<Exception>(() =>_commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, x => x.IsIdentificationRequired = FakeValues.IsFalse)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Null_Action_Returns_Command()
        {
            var input = TestObservationCreateInput();

            var commandFactory = ServiceLocator.Current.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(commandFactory);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, null);

            Assert.IsInstanceOf<ObservationCreateCommand>(command);
        }

        [Test, Category(TestCategories.Integration)]
        public void CommandBuilder_Build_Calls_Factory_Make()
        {
            var input = TestObservationCreateInput();

            var mockCommandFactory = new Mock<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(mockCommandFactory.Object);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, null);

            mockCommandFactory.Verify(x => x.Make(It.IsAny<ObservationCreateInput>()), Times.Once());
        }

        [Test, Category(TestCategories.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Action_Invokes_Action()
        {
            var input = TestObservationCreateInput();

            var commandFactory = ServiceLocator.Current.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(commandFactory);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, x => x.IsIdentificationRequired = !x.IsIdentificationRequired);

            Assert.AreNotEqual(input.IsIdentificationRequired, command.IsIdentificationRequired);
        }

        #endregion					
    }
}