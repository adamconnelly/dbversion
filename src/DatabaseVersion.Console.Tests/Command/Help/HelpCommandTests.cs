namespace dbversion.Console.Tests.Command.Help
{
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
            var messageService = new Mock<IMessageService>();

            createCommand.Setup(c => c.Name).Returns("create");
            createCommand.Setup(c => c.Description).Returns("Creates or upgrades a database.");

            versionCommand.Setup(c => c.Name).Returns("version");
            versionCommand.Setup(c => c.Description).Returns("Displays the application version.");

            helpCommand.Setup(c => c.Name).Returns("help");
            helpCommand.Setup(c => c.Description).Returns("Provides help on using the application.");

            command.Commands = new[] { createCommand.Object, versionCommand.Object, helpCommand.Object };
            command.MessageService = messageService.Object;

            // Act
            command.Execute(new[] { "help" });

            // Assert
            messageService.Verify(m => m.WriteLine("  create   Creates or upgrades a database."));
            messageService.Verify(m => m.WriteLine("  help     Provides help on using the application."));
            messageService.Verify(m => m.WriteLine("  version  Displays the application version."));
        }
    }
}

