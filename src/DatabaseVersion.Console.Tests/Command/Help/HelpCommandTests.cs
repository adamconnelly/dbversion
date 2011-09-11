namespace dbversion.Console.Tests.Command.Help
{
    using System;

    using dbversion.Console.Command;
    using dbversion.Console.Command.Help;

    using Moq;

    using Xunit;

    public class HelpCommandTests
    {
        [Fact]
        public void ShouldPrintListOfCommandsAndDescriptions()
        {
            // Arrange
            var command = new HelpCommand();
            var createCommand = new Mock<IConsoleCommand>();
            var versionCommand = new Mock<IConsoleCommand>();
            var helpCommand = new Mock<IConsoleCommand>();
            var messageService = new MessageServiceMock();

            createCommand.Setup(c => c.Name).Returns("create");
            createCommand.Setup(c => c.Description).Returns("Creates or upgrades a database.");

            versionCommand.Setup(c => c.Name).Returns("version");
            versionCommand.Setup(c => c.Description).Returns("Displays the application version.");

            helpCommand.Setup(c => c.Name).Returns("help");
            helpCommand.Setup(c => c.Description).Returns("Provides help on using the application.");

            command.Commands = new[] { createCommand.Object, versionCommand.Object, helpCommand.Object };
            command.MessageService = messageService;

            // Act
            command.Execute(new[] { "help" });

            // Assert
            string expected =
                "  create   Creates or upgrades a database." + Environment.NewLine +
                "  help     Provides help on using the application." + Environment.NewLine +
                "  version  Displays the application version." + Environment.NewLine + Environment.NewLine +
                "  Use dbversion help [command] for more help on a command." + Environment.NewLine;
            Assert.Equal(expected, messageService.Contents);
        }

        [Fact]
        public void ShouldDisplayCommandDescriptionWhenCommandIsSpecified()
        {
            // Arrange
            var command = new HelpCommand();
            var createCommand = new Mock<IConsoleCommand>();
            var messageService = new MessageServiceMock();

            createCommand.Setup(c => c.Name).Returns("create");
            createCommand.Setup(c => c.Description).Returns("Creates or upgrades a database");
            createCommand.Setup(c => c.Usage).Returns("dbversion create [options]");
            createCommand.Setup(c => c.Parameters).Returns(
                new[]
                {
                    new CommandParameter("-a", "--archive", "Specifies the path to the archive."),
                    new CommandParameter("-c", "--connectionString", "Specifies the database connection string.")
                });

            command.Commands = new[] { createCommand.Object };
            command.MessageService = messageService;

            // Act
            command.Execute(new[] { "help", "create" });

            // Assert
            string expected =
                "Usage: dbversion create [options]" + Environment.NewLine + Environment.NewLine +
                "Options:" + Environment.NewLine +
                "  -a, --archive           Specifies the path to the archive." + Environment.NewLine +
                "  -c, --connectionString  Specifies the database connection string." + Environment.NewLine;
            Assert.Equal(expected, messageService.Contents);
        }

        [Fact]
        public void ShouldOnlyDisplayCommandUsageIfTheCommandHasNoParameters()
        {
            // Arrange
            var command = new HelpCommand();
            var createCommand = new Mock<IConsoleCommand>();
            var messageService = new MessageServiceMock();

            createCommand.Setup(c => c.Name).Returns("create");
            createCommand.Setup(c => c.Description).Returns("Creates or upgrades a database");
            createCommand.Setup(c => c.Usage).Returns("dbversion create [options]");

            command.Commands = new[] { createCommand.Object };
            command.MessageService = messageService;

            // Act
            command.Execute(new[] { "help", "create" });

            // Assert
            string expected =
                "Usage: dbversion create [options]" + Environment.NewLine;
            Assert.Equal(expected, messageService.Contents);
        }

        [Fact]
        public void ShouldDisplayErrorMessageAndUsageIfCommandIsUnknown()
        {
            // Arrange
            var command = new HelpCommand();
            var createCommand = new Mock<IConsoleCommand>();
            var messageService = new MessageServiceMock();

            createCommand.Setup(c => c.Name).Returns("create");

            command.Commands = new[] { createCommand.Object };
            command.MessageService = messageService;

            // Act
            command.Execute(new[] { "help", "some-command" });

            // Assert
            string expected =
                "Unknown command 'some-command'." + Environment.NewLine + Environment.NewLine +
                "Usage: dbversion help [command]" + Environment.NewLine;
            Assert.Equal(expected, messageService.Contents);
        }
    }
}

