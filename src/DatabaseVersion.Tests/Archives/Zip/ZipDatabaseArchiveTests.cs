namespace dbversion.Tests.Archives.Zip
{
    using System.IO;
    using System.Linq;

    using dbversion.Archives;
    using dbversion.Archives.Zip;
    using dbversion.Manifests;
    using dbversion.Tests.Utils;
    using dbversion.Utils;
    using dbversion.Version;
    using dbversion.Property;

    using Ionic.Zip;

    using Moq;

    using Xunit;

    public class ZipDatabaseArchiveTests
    {
        [Fact]
        public void ShouldCreateArchivesUsingManifestReader()
        {
            using (ZipFile zipFile = new ZipFile())
            {
                // Arrange
                string path = Path.GetTempFileName();
                var tempDir = FileUtil.CreateTempDirectory();
                var tempFileName = Path.Combine(tempDir.FullName, "database.xml");
                System.IO.File.Create(tempFileName).Dispose();
                zipFile.AddFile(tempFileName, "1");
                zipFile.AddFile(tempFileName, "2");
                zipFile.Save(path);

                Mock<IManifestReader > manifestReader = new Mock<IManifestReader>();
                manifestReader.Setup(r => r.Read(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IDatabaseArchive>())).Returns(new Mock<IDatabaseVersion>().Object);

                // Act
                ZipDatabaseArchive archive = new ZipDatabaseArchive(path, manifestReader.Object);

                // Assert
                Assert.Equal(2, archive.Versions.Count());
            }
        }

        [Fact]
        public void ShouldLoadPropertiesFromArchive()
        {
            // Arrange
            string path = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                var tempDir = FileUtil.CreateTempDirectory();
                var propertiesFileName = Path.Combine(tempDir.FullName, "properties.xml");
                using (var stream = File.Create(propertiesFileName))
                {
                    var propertyCollection = new PropertyCollection();
                    propertyCollection.Properties.Add(new Property { Key = "property1", Value = "property1.value" });
                    propertyCollection.Properties.Add(new Property { Key = "property2", Value = "property2.value" });
                    XmlSerializer.Serialize(propertyCollection).CopyTo(stream);
                }

                zipFile.AddFile(propertiesFileName, "/");
                zipFile.Save(path);
            }

            var archive = new ZipDatabaseArchive(path, new Mock<ManifestReader>().Object);

            // Act
            var properties = archive.Properties;

            // Assert
            Assert.Equal("property1.value", properties.Single(p => p.Key == "property1").Value);
            Assert.Equal("property2.value", properties.Single(p => p.Key == "property2").Value);
        }

        [Fact]
        public void ShouldHaveEmptyPropertiesIfNoPropertiesFileExists()
        {
            // Arrange
            string path = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                zipFile.Save(path);
            }

            var archive = new ZipDatabaseArchive(path, new Mock<ManifestReader>().Object);

            // Act
            var properties = archive.Properties;

            // Assert
            Assert.Empty(properties);
        }
    }
}
