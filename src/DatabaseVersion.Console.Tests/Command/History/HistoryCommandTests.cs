namespace dbversion.Console.Tests.Command.History
{
    using dbversion.Console.Command.History;

    using Moq;

    using Xunit;

    public class HistoryCommandTests
    {
        [Fact]
        public void ShouldHaveCorrectCommandName()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("history", name);
        }

        [Fact]
        public void ShouldHaveCorrectDescription()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var description = command.Description;

            // Assert
            Assert.Equal("Prints out a history of the installed versions.", description);
        }

        [Fact]
        public void ShouldHaveCorrectCommandVersion()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var usage = command.Usage;

            // Assert
            Assert.Equal(string.Format("dbversion {0} [options]", command.Name), usage);
        }

        [Fact]
        public void ShouldUseVersionProviderToGetCurrentVersion()
        {
            // Arrange
//            var command = new HistoryCommand();
//            var versionProvider = new Mock<IVersionProvider>();
//            versionProvider.Setup(v => v.GetCurrentVersion(It.IsAny<ISession>())).Returns(new ClassicVersion("1.23"));
            
            // Act

            // Assert
        }
    }

}

