namespace dbversion.Console.Tests.Command.Create
{
    using System.Collections.Generic;

    using dbversion;
    using dbversion.Archives;
    using dbversion.Connections;
    using dbversion.Console.Command.Create;
    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;
    using dbversion.Tasks;
    using dbversion.Version;

    using Moq;

    using Xunit;

    public class CreateCommandTests
    {
        #region Fields

        private readonly Mock<IMessageService> messageService = new Mock<IMessageService>();
        private readonly Mock<IPropertyService> propertyService = new Mock<IPropertyService>();
        private readonly Mock<IDatabaseCreator> creator = new Mock<IDatabaseCreator>();
        private readonly Mock<IDatabaseArchive> archive = new Mock<IDatabaseArchive>();
        private readonly Mock<IDatabaseArchiveFactory> archiveFactory = new Mock<IDatabaseArchiveFactory>();
        private readonly Mock<ISettingsService> settingsService = new Mock<ISettingsService>();
        private readonly Mock<ISavedConnectionService> savedConnectionService = new Mock<ISavedConnectionService>();

        #endregion

        [Fact]
        public void ShouldHaveCorrectCommandName()
        {
            // Arrange
            var command = new CreateCommand();

            // Act
            string name = command.Name;

            // Assert
            Assert.Equal("create", name);
        }

        [Fact]
        public void ShouldUseDatabaseCreatorToCreateDatabase()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive", "-v", "12345" });

            // Assert
            creator.Verify(c => c.Create(archive.Object, "12345", It.Is<ITaskExecuter>(t => t.GetType() == typeof(ConsoleTaskExecuter)), It.Is<bool>(t => t)));
        }

        [Fact]
        public void ShouldPrintMessageIfArchiveIsNotSpecified()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new string[] { });

            // Assert
            messageService.Verify(m => m.WriteLine("Please specify an archive using the -a switch"));
        }

        [Fact]
        public void ShouldPrintMessageIfArchiveIsEmptyString()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", string.Empty });

            // Assert
            messageService.Verify(m => m.WriteLine("Please specify an archive using the -a switch"));
        }

        [Fact]
        public void ShouldPrintMessageIfArchiveIsNotSupported()
        {
            // Arrange
            var command = this.CreateCommand();
            command.ArchiveFactories = new[] { this.archiveFactory.Object };
            this.archiveFactory.Setup(f => f.Create(It.IsAny<string>())).Returns((IDatabaseArchive)null);

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            messageService.Verify(m => m.WriteLine("The specified archive is not supported"));
        }

        #region Connection String Property

        [Fact]
        public void ShouldSetConnectionStringProperty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive", "-c", "connectionString" });

            // Assert
            this.propertyService.Verify(
                p => p.Add(
                    new Property { Key = SessionFactoryProvider.ConnectionStringProperty, Value = "connectionString" }));
        }

        [Fact]
        public void ShouldNotSetConnectionStringPropertyIfItIsNullOrEmpty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive" });
            command.Execute(new[] { "-a", "myArchive", "-c", string.Empty });

            // Assert
            this.propertyService.Verify(
                p => p.Add(It.Is<Property>(pr => pr.Key == SessionFactoryProvider.ConnectionStringProperty)), Times.Never());
        }

        #endregion

        #region Provider Property

        [Fact]
        public void ShouldSetProviderProperty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive", "-p", "provider" });

            // Assert
            this.propertyService.Verify(
                p => p.Add(
                    new Property { Key = SessionFactoryProvider.ProviderProperty, Value = "provider" }));
        }

        [Fact]
        public void ShouldNotSetProviderPropertyIfItIsNullOrEmpty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive" });
            command.Execute(new[] { "-a", "myArchive", "-p", string.Empty });

            // Assert
            this.propertyService.Verify(
                p => p.Add(It.Is<Property>(pr => pr.Key == SessionFactoryProvider.ProviderProperty)), Times.Never());
        }

        #endregion

        #region Driver Class Property

        [Fact]
        public void ShouldSetDriverClassProperty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive", "-d", "driverClass" });

            // Assert
            this.propertyService.Verify(
                p => p.Add(
                    new Property { Key = SessionFactoryProvider.DriverClassProperty, Value = "driverClass" }));
        }

        [Fact]
        public void ShouldNotSetDriverClassPropertyIfItIsNullOrEmpty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive" });
            command.Execute(new[] { "-a", "myArchive", "-d", string.Empty });

            // Assert
            this.propertyService.Verify(
                p => p.Add(It.Is<Property>(pr => pr.Key == SessionFactoryProvider.DriverClassProperty)), Times.Never());
        }

        #endregion

        #region Dialect Property

        [Fact]
        public void ShouldSetDialectProperty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive", "-l", "dialect" });

            // Assert
            this.propertyService.Verify(
                p => p.Add(
                    new Property { Key = SessionFactoryProvider.DialectProperty, Value = "dialect" }));
        }

        [Fact]
        public void ShouldNotSetDialectPropertyIfItIsNullOrEmpty()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive" });
            command.Execute(new[] { "-a", "myArchive", "-l", string.Empty });

            // Assert
            this.propertyService.Verify(
                p => p.Add(It.Is<Property>(pr => pr.Key == SessionFactoryProvider.DialectProperty)), Times.Never());
        }

        #endregion

        [Fact]
        public void ShouldSetDefaultPropertyValues()
        {
            // Arrange
            var command = this.CreateCommand();

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            this.propertyService.Verify(p => p.SetDefaultProperties());
        }

        [Fact]
        public void ShouldMergeArchiveProperties()
        {
            // Arrange
            var command = this.CreateCommand();

            var properties = new[] { new Property { Key = "key", Value = "value" }};
            archive.Setup(a => a.Properties).Returns(properties);

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            this.propertyService.Verify(p => p.Merge(properties));
        }

        [Fact]
        public void ShouldMergePropertiesFromSettings()
        {
            // Arrange
            var command = this.CreateCommand();

            var propertyCollection = new PropertyCollection();
            propertyCollection.Properties.Add(new Property { Key = "key", Value = "value" });

            settingsService.Setup(s => s.DeSerialize<PropertyCollection>(It.IsAny<string>())).Returns(propertyCollection);

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            this.propertyService.Verify(p => p.Merge(propertyCollection.Properties));
        }

        [Fact]
        public void ShouldPrintExceptionMessageIfVersionNotFoundExceptionIsThrownByCreator()
        {
            // Arrange
            var command = this.CreateCommand();
            var exception = new VersionNotFoundException("12345");

            this.creator.Setup(
                c => c.Create(It.IsAny<IDatabaseArchive>(), It.IsAny<string>(), It.IsAny<ITaskExecuter>(), It.Is<bool>(t => t)))
                .Throws(exception);

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            this.messageService.Verify(m => m.WriteLine(exception.Message));
        }

        [Fact]
        public void ShouldReturnFalseIfVersionNotFoundExceptionIsThrownByCreator()
        {
            // Arrange
            var command = this.CreateCommand();
            var exception = new VersionNotFoundException("12345");

            this.creator.Setup(
                c => c.Create(It.IsAny<IDatabaseArchive>(), It.IsAny<string>(), It.IsAny<ITaskExecuter>(), It.Is<bool>(t => t)))
                .Throws(exception);

            // Act
            var result = command.Execute(new[] { "-a", "myArchive" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldPrintExceptionMessageIfTaskExecutionExceptionIsThrownByCreator()
        {
            // Arrange
            var command = this.CreateCommand();
            var exception = new TaskExecutionException("The task failed to execute");

            this.creator.Setup(
                c => c.Create(It.IsAny<IDatabaseArchive>(), It.IsAny<string>(), It.IsAny<ITaskExecuter>(), It.Is<bool>(t => t)))
                .Throws(exception);

            // Act
            command.Execute(new[] { "-a", "myArchive" });

            // Assert
            this.messageService.Verify(m => m.WriteLine(exception.Message));
        }

        [Fact]
        public void ShouldReturnFalseIfTaskExecutionExceptionIsThrownByCreator()
        {
            // Arrange
            var command = this.CreateCommand();
            var exception = new TaskExecutionException("The task failed to execute");

            this.creator.Setup(
                c => c.Create(It.IsAny<IDatabaseArchive>(), It.IsAny<string>(), It.IsAny<ITaskExecuter>(), It.Is<bool>(t => t)))
                .Throws(exception);

            // Act
            var result = command.Execute(new[] { "-a", "myArchive" });

            // Assert
            Assert.False(result);
        }

        private CreateCommand CreateCommand()
        {
            var command = new CreateCommand();
            command.Creator = this.creator.Object;
            command.ArchiveFactories = new[] { this.archiveFactory.Object };
            command.SettingsService = this.settingsService.Object;
            command.PropertyService = this.propertyService.Object;
            command.MessageService = this.messageService.Object;
            command.SavedConnectionService = this.savedConnectionService.Object;

            archiveFactory.Setup(f => f.CanCreate("myArchive")).Returns(true);
            archiveFactory.Setup(f => f.Create("myArchive")).Returns(archive.Object);

            return command;
        }
    }
}
