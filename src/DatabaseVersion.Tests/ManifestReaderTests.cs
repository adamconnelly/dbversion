using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.IO;
using System.Reflection;
using Moq;
using System.Xml;
using System.Xml.Linq;

namespace DatabaseVersion.Tests
{
    public class ManifestReaderTests
    {
        private const string ManifestPath = "DatabaseVersion.Tests.ExampleManifests.example.xml";
        private const string OneTaskManifestPath = "DatabaseVersion.Tests.ExampleManifests.oneTask.xml";

        [Fact]
        public void ShouldSetVersionFromManifest()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath);

            // Assert
            Assert.Equal("14", version.Version);
        }

        [Fact]
        public void ShouldSetManifestPath()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath);

            // Assert
            Assert.Equal(ManifestPath, version.ManifestPath);
        }

        [Fact]
        public void ShouldThrowExceptionIfManifestStreamIsNull()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            // Act
            Exception exception = Record.Exception(() => reader.Read(null, ManifestPath));
            
            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfManifestPathIsNull()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            // Act
            Exception exception = Record.Exception(() => reader.Read(GetManifest(), null));

            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldUseTaskFactoryToCreateTasks()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            Mock<IDatabaseTaskFactory> factory = new Mock<IDatabaseTaskFactory>();
            Mock<IDatabaseTask> task = new Mock<IDatabaseTask>();
            factory.Setup(f => f.CanCreate(It.Is<XElement>(r => r.Name == "script"))).Returns(true);
            factory.Setup(f => f.Create(It.Is<XElement>(r => r.Name == "script"), 0)).Returns(task.Object);

            reader.Factories = new[] { factory.Object };

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(OneTaskManifestPath), OneTaskManifestPath);

            // Assert
            Assert.Same(task.Object, version.Tasks.Single());
        }

        [Fact]
        public void ShouldCreateTasksWithCorrectExecutionOrder()
        {
            // Arrange
            ManifestReader reader = new ManifestReader();

            Mock<IDatabaseTaskFactory> factory = new Mock<IDatabaseTaskFactory>();
            Mock<IDatabaseTask> task = new Mock<IDatabaseTask>();
            factory.Setup(f => f.CanCreate(It.IsAny<XElement>())).Returns(true);
            factory.Setup(f => f.Create(It.IsAny<XElement>(), It.IsAny<int>())).Returns(task.Object);

            reader.Factories = new[] { factory.Object };

            // Act
            IDatabaseVersion version = reader.Read(GetManifest(), ManifestPath);

            // Assert
            int expectedOrder = 0;
            foreach (IDatabaseTask createdTask in version.Tasks)
            {
                factory.Verify(f => f.Create(It.IsAny<XElement>(), expectedOrder));
                expectedOrder++;
            }
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
    }
}
