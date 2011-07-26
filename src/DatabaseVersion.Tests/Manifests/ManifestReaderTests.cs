using System;
using System.Linq;
using DatabaseVersion.Version.NumericVersion;
using Xunit;
using System.IO;
using System.Reflection;
using Moq;
using System.Xml.Linq;
using DatabaseVersion.Archives;
using DatabaseVersion.Manifests;
using DatabaseVersion.Tasks;

namespace DatabaseVersion.Tests.Manifests
{
    public class ManifestReaderTests
    {
        private const string ManifestPath = "DatabaseVersion.Tests.ExampleManifests.example.xml";
        private const string OneTaskManifestPath = "DatabaseVersion.Tests.ExampleManifests.oneTask.xml";

        private readonly Mock<IDatabaseArchive> databaseArchive = new Mock<IDatabaseArchive>();
        private readonly Mock<IVersionProvider> versionProvider = new Mock<IVersionProvider>();

        [Fact]
        public void ShouldUseProviderToCreateVersion()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();
            NumericVersion numericVersion = new NumericVersion(14);
            versionProvider.Setup(v => v.CreateVersion("14")).Returns(numericVersion);

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath, databaseArchive.Object);

            // Assert
            Assert.Equal(numericVersion, version.Version);
        }

        [Fact]
        public void ShouldSetManifestPath()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath, databaseArchive.Object);

            // Assert
            Assert.Equal(ManifestPath, version.ManifestPath);
        }

        [Fact]
        public void ShouldThrowExceptionIfManifestStreamIsNull()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            // Act
            Exception exception = Record.Exception(() => reader.Read(null, ManifestPath, databaseArchive.Object));
            
            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfManifestPathIsNull()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            // Act
            Exception exception = Record.Exception(() => reader.Read(GetManifest(), null, databaseArchive.Object));

            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfDatabaseArchiveIsNull()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            // Act
            Exception exception = Record.Exception(() => reader.Read(GetManifest(), ManifestPath, null));

            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldUseTaskFactoryToCreateTasks()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            Mock<IDatabaseTaskFactory> factory = new Mock<IDatabaseTaskFactory>();
            Mock<IDatabaseTask> task = new Mock<IDatabaseTask>();
            factory.Setup(f => f.CanCreate(It.Is<XElement>(r => r.Name == "script"))).Returns(true);
            factory.Setup(f => f.Create(It.Is<XElement>(r => r.Name == "script"), 0, It.IsAny<IDatabaseVersion>())).Returns(task.Object);

            reader.Factories = new[] { factory.Object };

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(OneTaskManifestPath), OneTaskManifestPath, this.databaseArchive.Object);

            // Assert
            Assert.Same(task.Object, version.Tasks.Single());
        }

        [Fact]
        public void ShouldCreateTasksWithCorrectExecutionOrder()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            Mock<IDatabaseTaskFactory> factory = new Mock<IDatabaseTaskFactory>();
            Mock<IDatabaseTask> task = new Mock<IDatabaseTask>();
            factory.Setup(f => f.CanCreate(It.IsAny<XElement>())).Returns(true);
            factory.Setup(f => f.Create(It.IsAny<XElement>(), It.IsAny<int>(), It.IsAny<IDatabaseVersion>())).Returns(task.Object);

            reader.Factories = new[] { factory.Object };

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath, this.databaseArchive.Object);

            // Assert
            int expectedOrder = 0;
            foreach (IDatabaseTask createdTask in version.Tasks)
            {
                factory.Verify(f => f.Create(It.IsAny<XElement>(), expectedOrder, It.IsAny<IDatabaseVersion>()));
                expectedOrder++;
            }
        }

        [Fact]
        public void ShouldSetDatabaseArchiveInReturnedVersion()
        {
            // Arrange
            ManifestReader reader = this.CreateManifestReader();

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath, databaseArchive.Object);

            // Assert
            Assert.Equal(databaseArchive.Object, version.Archive);
        }

        private Stream GetManifest()
        {
            return GetManifest(ManifestPath);
        }

        private Stream GetManifest(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(path);
        }

        private ManifestReader CreateManifestReader()
        {
            ManifestReader reader = new ManifestReader();
            reader.VersionProvider = versionProvider.Object;
            return reader;
        }
    }
}
