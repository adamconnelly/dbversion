namespace dbversion.Console.Tests.Command
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Archives;
    using Connections;
    using Console.Command.Check;
    using Moq;
    using NHibernate;
    using Property;
    using Session;
    using Settings;
    using Tasks;
    using Xunit;
    using dbversion.Version;
    using dbversion.Version.ClassicVersion;

    public class CheckCommandTests
    {
        private readonly MessageServiceMock messageService = new MessageServiceMock();
        private readonly Mock<ISavedConnectionService> savedConnectionService = new Mock<ISavedConnectionService>();
        private readonly Mock<IPropertyService> propertyService = new Mock<IPropertyService>();
        private readonly Mock<ISettingsService> settingsService = new Mock<ISettingsService>();
        private readonly Mock<IDatabaseArchiveFactory> archiveFactory = new Mock<IDatabaseArchiveFactory>();
        private readonly Mock<IDatabaseArchive> archive = new Mock<IDatabaseArchive>();
        private readonly Mock<IVersionProvider> versionProvider = new Mock<IVersionProvider>();
        private readonly Mock<ISessionFactoryProvider> sessionFactoryProvider = new Mock<ISessionFactoryProvider>();
        private readonly Mock<ISessionFactory> sessionFactory = new Mock<ISessionFactory>();
        private readonly Mock<ISession> session = new Mock<ISession>();

        [Fact]
        public void ShouldIndicateWhenAllScriptsHaveBeenRun()
        {
            // Arrange
            var command = CreateCommand();

            var versions = new[] {CreateVersionWithTask().Object, CreateVersionWithTask().Object};
            archive.Setup(a => a.Versions).Returns(versions);

            versionProvider.Setup(
                p => p.HasExecutedScript(It.IsAny<VersionBase>(), It.IsAny<VersionBase>(), It.IsAny<IDatabaseTask>()))
                           .Returns(true);
            versionProvider.Setup(v => v.GetVersion(It.IsAny<ISession>(), It.IsAny<VersionBase>()))
                           .Returns(new ClassicVersion());

            // Act
            command.Execute(new string[] {});

            // Assert
            Assert.Equal("The database is up to date.", this.messageService.Contents.TrimEnd());
        }

        [Fact]
        public void ShouldIndicateIfACertainScriptHasNotBeenRun()
        {
            // Arrange
            var command = CreateCommand();

            var version1 = CreateVersionWithTask("1.1", "Task 1");
            var task1 = version1.Object.Tasks.First();
            archive.Setup(a => a.Versions).Returns(new[] {version1.Object});

            versionProvider.Setup(
                p =>
                p.HasExecutedScript(It.IsAny<VersionBase>(), It.IsAny<VersionBase>(), task1))
                           .Returns(false);

            versionProvider.Setup(v => v.GetVersion(It.IsAny<ISession>(), It.IsAny<VersionBase>()))
                           .Returns(new ClassicVersion());

            // Act
            command.Execute(new string[] {});

            // Assert
            string expectedMessage = "Version 1.1 missing:" + Environment.NewLine +
                "  Task 1";
            Assert.Equal(expectedMessage, this.messageService.Contents.TrimEnd());
        }

        [Fact]
        public void ShouldRetrieveCurrentVersionFromDatabaseForComparison()
        {
            // Arrange
            var command = CreateCommand();

            var version1 = CreateVersionWithTask("1.1", "Task 1");
            archive.Setup(a => a.Versions).Returns(new[] { version1.Object });

            var versionObject = version1.Object.Version;

            var expectedVersion = new ClassicVersion("1.1");
            versionProvider.Setup(p => p.GetVersion(this.session.Object, versionObject)).Returns(expectedVersion);

            // Act
            command.Execute(new string[] { });

            // Assert
            VersionBase actualVersion = null;
            versionProvider.Verify(
                v =>
                v.HasExecutedScript(It.Is<VersionBase>(vo => CatchVersion(vo, out actualVersion)), It.IsAny<VersionBase>(),
                                    It.IsAny<IDatabaseTask>()));

            Assert.Same(expectedVersion, actualVersion);
        }

        [Fact]
        public void ShouldOnlyOutputInformationForSpecifiedVersion()
        {
            // Arrange
            var command = CreateCommand();

            var version1 = CreateVersionWithTask("1.1", "Task 1");
            var version2 = CreateVersionWithTask("1.2", "Task 1");
            archive.Setup(a => a.Versions).Returns(new[] { version1.Object, version2.Object });

            var versionObject = version1.Object.Version;

            var expectedVersion = new ClassicVersion("1.1");
            versionProvider.Setup(p => p.GetVersion(this.session.Object, versionObject)).Returns(expectedVersion);
            versionProvider.Setup(p => p.CreateVersion(versionObject.VersionText)).Returns(versionObject);

            // Act
            command.Execute(new [] { "-v", "1.1" });

            // Assert
            string expectedText = "Version 1.1 missing:" + Environment.NewLine +
                "  Task 1";
            Assert.Equal(expectedText, this.messageService.Contents.TrimEnd());
        }

        [Fact]
        public void ShouldIndicateIfSpecifiedVersionDoesNotExist()
        {
            // Arrange
            var command = CreateCommand();

            var version1 = CreateVersionWithTask("1.1", "Task 1");
            archive.Setup(a => a.Versions).Returns(new[] { version1.Object });

            var versionObject = new ClassicVersion("1.2");
            versionProvider.Setup(p => p.CreateVersion(versionObject.VersionText)).Returns(versionObject);

            // Act
            var result = command.Execute(new[] { "-v", "1.2" });

            // Assert
            Assert.Equal("Version 1.2 does not exist in the archive.", this.messageService.Contents.TrimEnd());
            Assert.False(result);
        }

        [Fact]
        public void ShouldIndicateIfASpecificVersionHasNotBeenInstalledInTheDatabase()
        {
            // Arrange
            var command = CreateCommand();

            archive.Setup(a => a.Versions)
                   .Returns(new[]
                       {
                           CreateVersionWithTask("1.1", "Task 1").Object, CreateVersionWithTask("1.2", "Task 1").Object
                       });

            versionProvider.Setup(p => p.GetVersion(this.session.Object, It.IsAny<VersionBase>())).Returns((VersionBase)null);

            // Act
            command.Execute(new string[] { });

            // Assert
            var expectedMessage = "Version 1.1 has not been installed." + Environment.NewLine +
                "Version 1.2 has not been installed.";
            Assert.Equal(expectedMessage, this.messageService.Contents.TrimEnd());
        }

        [Fact]
        public void ShouldIndicateIfNoArchiveHasBeenSpecified()
        {
            // Arrange
            var command = CreateCommand();

            this.archiveFactory.Setup(a => a.Create(It.IsAny<string>())).Returns((IDatabaseArchive) null);

            // Act
            var result = command.Execute(new string[] { });

            // Assert
            Assert.Equal("Please specify an archive using the -a switch.",
                         this.messageService.Contents.TrimEnd());
            Assert.False(result);
        }

        private bool CatchVersion(VersionBase input, out VersionBase actualVersion)
        {
            actualVersion = input;
            return true;
        }

        private CheckCommand CreateCommand()
        {
            archiveFactory.Setup(s => s.CanCreate(It.IsAny<string>())).Returns(true);
            archiveFactory.Setup(s => s.Create(It.IsAny<string>())).Returns(archive.Object);

            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());

            this.sessionFactoryProvider.Setup(p => p.CreateSessionFactory()).Returns(this.sessionFactory.Object);
            this.sessionFactory.Setup(s => s.OpenSession()).Returns(session.Object);

            var command = new CheckCommand
                {
                    SavedConnectionService = this.savedConnectionService.Object,
                    PropertyService = this.propertyService.Object,
                    SettingsService = this.settingsService.Object,
                    MessageService = this.messageService,
                    VersionProvider = this.versionProvider.Object,
                    ArchiveFactories = new[] { this.archiveFactory.Object },
                    SessionFactoryProvider = this.sessionFactoryProvider.Object
                };

            return command;
        }

        private static Mock<IDatabaseVersion> CreateVersionWithTask()
        {
            return CreateVersionWithTask("0.0", null);
        }

        private static Mock<IDatabaseVersion> CreateVersionWithTask(string versionString, string fileName)
        {
            var classicVersion = new ClassicVersion(versionString);
            var databaseVersion = new Mock<IDatabaseVersion>();

            databaseVersion.Setup(v => v.Version).Returns(classicVersion);

            var task = new Mock<IDatabaseTask>();
            task.Setup(t => t.FileName).Returns(fileName);
            databaseVersion.Setup(v => v.Tasks).Returns(new[] { task.Object });

            return databaseVersion;
        }
    }
}
