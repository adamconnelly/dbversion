using Xunit;
using Ionic.Zip;
using Moq;
using DatabaseVersion.Manifests;
using DatabaseVersion.Archives.Zip;
using System.IO;
using System.Linq;
using DatabaseVersion.Tests.Utils;
using DatabaseVersion.Archives;
namespace DatabaseVersion.Tests.Archives.Zip
{
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

                Mock<IManifestReader> manifestReader = new Mock<IManifestReader>();
                manifestReader.Setup(r => r.Read(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IDatabaseArchive>())).Returns(new Mock<IDatabaseVersion>().Object);

                // Act
                ZipDatabaseArchive archive = new ZipDatabaseArchive(path, manifestReader.Object);

                // Assert
                Assert.Equal(2, archive.Versions.Count());
            }
        }
    }
}
