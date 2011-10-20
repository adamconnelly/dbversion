namespace dbversion.Console.Tests.Command.SavedConnection
{
    using System;
    using System.Collections.Generic;

    using dbversion.Connections;

    using dbversion.Console.Command.SavedConnection;

    using Moq;

    using Xunit;

    public class SavedConnectionCommandTests
    {
        [Fact]
        public void ShouldHaveCorrectCommandName()
        {
            // Arrange
            var command = new SavedConnectionCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("saved-connection", name);
        }

        [Fact]
        public void ShouldHaveCorrectCommandDescription()
        {
            // Arrange
            var command = new SavedConnectionCommand();

            // Act
            var description = command.Description;

            // Assert
            Assert.Equal("Allows connection details to be saved and managed.", description);
        }

        [Fact]
        public void ShouldListAllConnectionsWhenNoArgumentsAreSpecified()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());

            // Act
            command.Execute(new[] { "saved-connection" });

            // Assert
            var expectedMessage = "connection2:" + Environment.NewLine +
                "  Connection String: connString2" + Environment.NewLine +
                "  Driver Class: driver2" + Environment.NewLine +
                "  Provider: provider2" + Environment.NewLine +
                "  Dialect: dialect2" + Environment.NewLine +
                "  Default: True" + Environment.NewLine + Environment.NewLine +
                "connection1:" + Environment.NewLine +
                "  Connection String: connString" + Environment.NewLine +
                "  Driver Class: driver" + Environment.NewLine +
                "  Provider: provider" + Environment.NewLine +
                "  Dialect: dialect" + Environment.NewLine +
                "  Default: False" + Environment.NewLine + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
        }

        [Fact]
        public void ShouldDisplayMessageWhenNoSavedConnectionsExist()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            // Act
            command.Execute(new[] { "saved-connection" });

            // Assert
            var expectedMessage = "There are no saved connections." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
        }

        [Fact]
        public void ShouldListCorrectConnectionWhenConnectionNameIsSpecified()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());

            // Act
            command.Execute(new[] { "saved-connection", "-n", "connection1" });

            // Assert
            var expectedMessage = "connection1:" + Environment.NewLine +
                "  Connection String: connString" + Environment.NewLine +
                "  Driver Class: driver" + Environment.NewLine +
                "  Provider: provider" + Environment.NewLine +
                "  Dialect: dialect" + Environment.NewLine +
                "  Default: False" + Environment.NewLine + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
        }

        [Fact]
        public void ShouldDisplayMessageWhenSpecifiedConnectionDoesNotExist()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());

            // Act
            command.Execute(new[] { "saved-connection", "-n", "abc" });

            // Assert
            var expectedMessage = "There is no saved connection called \"abc\"." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
        }

        [Fact]
        public void ShouldBeAbleToCreateNewSavedConnection()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.CreateSavedConnection("connection1", "connString", "provider", "driver", "dialect"))
                .Returns(new SavedConnection("connection1", "connString", "provider", "driver", "dialect", true));

            // Act
            command.Execute(
                new[]
                {
                    "saved-connection",
                    "-n", "connection1",
                    "-c", "connString",
                    "-l", "dialect",
                    "-p", "provider",
                    "-d", "driver"
                });

            // Assert
            var expectedMessage = "Created new saved connection \"connection1\":" + Environment.NewLine +
                "  Connection String: connString" + Environment.NewLine +
                "  Driver Class: driver" + Environment.NewLine +
                "  Provider: provider" + Environment.NewLine +
                "  Dialect: dialect" + Environment.NewLine +
                "  Default: True" + Environment.NewLine + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
            service.Verify(s => s.CreateSavedConnection("connection1", "connString", "provider", "driver", "dialect"));
            service.Verify(s => s.SaveConnections());
        }

        [Fact]
        public void ShouldBeAbleToDeleteSavedConnection()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());
            service.Setup(s => s.DeleteConnection("connection1")).Returns(true);

            // Act
            command.Execute(new[] { "saved-connection", "-rn", "connection1" });

            // Assert
            var expectedMessage = "\"connection1\" has been deleted." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
            service.Verify(s => s.SaveConnections());
        }

        [Fact]
        public void ShouldPrintErrorMessageIfDeletingConnectionFails()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());
            service.Setup(s => s.DeleteConnection("connection1")).Returns(false);

            // Act
            command.Execute(new[] { "saved-connection", "-rn", "connection1" });

            // Assert
            var expectedMessage = "There is no saved connection called \"connection1\"." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
            service.Verify(s => s.SaveConnections(), Times.Never());
        }

        [Fact]
        public void ShouldBeAbleToDefaultSavedConnection()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());
            service.Setup(s => s.SetDefaultConnection("connection1")).Returns(true);

            // Act
            command.Execute(new[] { "saved-connection", "-n", "connection1", "--default" });

            // Assert
            var expectedMessage = "\"connection1\" is now the default connection." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
            service.Verify(s => s.SaveConnections());
        }

        [Fact]
        public void ShouldPrintAnErrorMessageIfDefaultingFails()
        {
            // Arrange
            var command = new SavedConnectionCommand();
            var service = new Mock<ISavedConnectionService>();
            var messageService = new MessageServiceMock();

            command.SavedConnectionService = service.Object;
            command.MessageService = messageService;

            service.Setup(s => s.SavedConnections).Returns(CreateConnections());
            service.Setup(s => s.SetDefaultConnection("connection5")).Returns(false);

            // Act
            command.Execute(new[] { "saved-connection", "-n", "connection5", "--default" });

            // Assert
            var expectedMessage = "There is no saved connection called \"connection5\"." + Environment.NewLine;
            Assert.Equal(expectedMessage, messageService.Contents);
            service.Verify(s => s.SaveConnections(), Times.Never());
        }

        private static IEnumerable<SavedConnection> CreateConnections()
        {
            return new[]
                {
                    new SavedConnection
                    {
                        Name = "connection1",
                        ConnectionProvider = "provider",
                        ConnectionString = "connString",
                        Dialect = "dialect",
                        DriverClass = "driver",
                        IsDefault = false
                    },
                    new SavedConnection
                    {
                        Name = "connection2",
                        ConnectionProvider = "provider2",
                        ConnectionString = "connString2",
                        Dialect = "dialect2",
                        DriverClass = "driver2",
                        IsDefault = true
                    }
                };
        }
    }
}

