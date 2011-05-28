using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using DatabaseVersion.Archives.File;
using DatabaseVersion.Tests.Utils;
using System.IO;
using System.Reflection;
using DatabaseVersion.Manifests;
using Moq;
using DatabaseVersion.Archives;

namespace DatabaseVersion.Tests.Archives.File
{
    public class FileDatabaseArchiveTests
    {
        #region GetFile
        [Fact]
        public void ShouldBeAbleToGetAFileIfItExists()
        {
            // Arrange
            var testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);
            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, null);
            Stream expectedStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("DatabaseVersion.Tests.TestArchive._1.data.authors.sql");

            // Act
            Stream stream = archive.GetFile("_1/data/authors.sql");

            // Assert
            Assert.Equal(StreamToString(expectedStream), StreamToString(stream));

            testDirectory.Delete(true);
        }

        [Fact]
        public void ShouldReturnNullFromGetFileIfFileDoesNotExist()
        {
            // Arrange
            FileDatabaseArchive archive = new FileDatabaseArchive("aDirectory", null);

            // Act
            Stream stream = archive.GetFile("_1/data/authors.sql");

            // Assert
            Assert.Null(stream);
        }
        #endregion

        [Fact]
        public void ShouldReturnCorrectNumberOfDatabaseVersions()
        {
            // Arrange
            var testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);

            Mock<IManifestReader> manifestReader = new Mock<IManifestReader>();
            manifestReader.Setup(
                m => m.Read(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IDatabaseArchive>()))
                .Returns(new Mock<IDatabaseVersion>().Object);

            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, manifestReader.Object);

            // Act
            var versions = archive.Versions;

            // Assert
            Assert.Equal(2, versions.Count());

            testDirectory.Delete(true);
        }

        [Fact]
        public void ShouldUseManifestReaderToCreateDatabaseVersions()
        {
            // Arrange
            var testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);

            DatabaseVersion version1 = new DatabaseVersion("1", "_1\\database.xml", null);
            DatabaseVersion version2 = new DatabaseVersion("2", "_2\\database.xml", null);

            Mock<IManifestReader> manifestReader = new Mock<IManifestReader>();
            manifestReader.Setup(
                m => m.Read(It.IsAny<Stream>(), testDirectory.FullName + "\\_1\\database.xml", It.IsAny<IDatabaseArchive>()))
                .Returns(version1);
            manifestReader.Setup(
                m => m.Read(It.IsAny<Stream>(), testDirectory.FullName + "\\_2\\database.xml", It.IsAny<IDatabaseArchive>()))
                .Returns(version2);

            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, manifestReader.Object);

            // Act
            var versions = archive.Versions;

            // Assert
            Assert.Contains(version1, versions);
            Assert.Contains(version2, versions);

            testDirectory.Delete(true);
        }

        private string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
