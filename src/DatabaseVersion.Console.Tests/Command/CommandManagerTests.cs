namespace dbversion.Console.Tests.Command
{
    using dbversion.Console.Command;

    using Moq;

    using Xunit;

    public class CommandManagerTests
    {
        private readonly Mock<IMessageService> messageService = new Mock<IMessageService>();

        [Fact]
        public void ShouldChooseCorrectCommandToExecute()
        {
            // Arrange
            var commandManager = new CommandManager();

            var createCommand = new Mock<IConsoleCommand>();
            var saveConnectionCommand = new Mock<IConsoleCommand>();

            createCommand.Setup(c => c.Name).Returns("create");
            saveConnectionCommand.Setup(c => c.Name).Returns("save-connection");

            commandManager.Commands = new[] { createCommand.Object, saveConnectionCommand.Object };

            // Act
            string[] arguments = new[] { "create", "-a", "/path/to/some/archive" };
            commandManager.Execute(arguments);

            // Assert
            createCommand.Verify(c => c.Execute(arguments));
            saveConnectionCommand.Verify(c => c.Execute(It.IsAny<string[]>()), Times.Never());
        }

        #region No Arguments

        [Fact]
        public void ShouldOutputUsageIfArgumentsAreNull()
        {
            // Arrange
            var commandManager = new CommandManager();
            commandManager.MessageService = this.messageService.Object;

            // Act
            commandManager.Execute(null);

            // Assert
            messageService.Verify(m => m.WriteLine("Usage: dbversion command [Options]"));
        }

        [Fact]
        public void ShouldOutputUsageIfArgumentsAreEmpty()
        {
            // Arrange
            var commandManager = new CommandManager();
            commandManager.MessageService = this.messageService.Object;

            // Act
            commandManager.Execute(new string[] { });

            // Assert
            messageService.Verify(m => m.WriteLine("Usage: dbversion command [Options]"));
        }

        #endregion

        [Fact]
        public void ShouldDisplayErrorMessageIfCommandIsUnknown()
        {
            // Arrange
            var commandManager = new CommandManager();
            var createCommand = new Mock<IConsoleCommand>();
            createCommand.Setup(c => c.Name).Returns("create");
            commandManager.Commands = new[] { createCommand.Object };
            commandManager.MessageService = this.messageService.Object;

            // Act
            string[] arguments = new[] { "-a", "/path/to/some/archive" };
            commandManager.Execute(arguments);

            // Assert
            createCommand.Verify(c => c.Execute(It.IsAny<string[]>()), Times.Never());
            this.messageService.Verify(m => m.WriteLine("Unknown command \"-a\"."));
        }
    }
}

