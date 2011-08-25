namespace dbversion.Tests.Archives.File
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using dbversion.Version.NumericVersion;
    using Xunit;
    using dbversion.Archives.File;
    using dbversion.Tests.Utils;
    using System.IO;
    using System.Reflection;
    using dbversion.Manifests;
    using Moq;
    using dbversion.Archives;
    using dbversion.Version;

    public class FileDatabaseArchiveTests : IDisposable
    {
        private readonly Mock<IManifestReader> manifestReader = new Mock<IManifestReader>();
        private DirectoryInfo testDirectory;

        #region GetFile
        [Fact]
        public void ShouldBeAbleToGetAFileIfItExists()
        {
            // Arrange
            this.testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);
            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, this.manifestReader.Object);
            Stream expectedStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("DatabaseVersion.Tests.TestArchive._1.data.authors.sql");

            // Act
            Stream stream = archive.GetFile("_1/data/authors.sql");

            // Assert
            Assert.Equal(StreamToString(expectedStream), StreamToString(stream));
        }

        [Fact]
        public void ShouldReturnNullFromGetFileIfFileDoesNotExist()
        {
            // Arrange
            this.testDirectory = FileUtil.CreateTempDirectory();
            FileDatabaseArchive archive = new FileDatabaseArchive(this.testDirectory.FullName, this.manifestReader.Object);

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
            this.testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);

            Mock<IManifestReader > manifestReader = new Mock<IManifestReader>();
            manifestReader.Setup(
                m => m.Read(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<IDatabaseArchive>()))
                .Returns(new Mock<IDatabaseVersion>().Object);

            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, manifestReader.Object);

            // Act
            var versions = archive.Versions;

            // Assert
            Assert.Equal(2, versions.Count());
        }

        [Fact]
        public void ShouldUseManifestReaderToCreateDatabaseVersions()
        {
            // Arrange
            this.testDirectory = FileUtil.CreateTempDirectory();
            AssemblyUtil.CopyContentsToDirectory("DatabaseVersion.Tests.TestArchive", testDirectory.FullName);

            DatabaseVersion version1 = new DatabaseVersion(new NumericVersion(1), "_1" + Path.DirectorySeparatorChar + "database.xml", null);
            DatabaseVersion version2 = new DatabaseVersion(new NumericVersion(2), "_2" + Path.DirectorySeparatorChar + "database.xml", null);
			
            this.manifestReader.Setup(
                m => m.Read(It.IsAny<Stream>(), 
				string.Format("{1}{0}{2}{0}{3}", Path.DirectorySeparatorChar, this.testDirectory.FullName, "_1", "database.xml"),
				It.IsAny<IDatabaseArchive>()))
                .Returns(version1);
            this.manifestReader.Setup(
				m => m.Read(It.IsAny<Stream>(),
                string.Format("{1}{0}{2}{0}{3}", Path.DirectorySeparatorChar, this.testDirectory.FullName, "_2", "database.xml"),
				It.IsAny<IDatabaseArchive>()))
                .Returns(version2);

            FileDatabaseArchive archive = new FileDatabaseArchive(testDirectory.FullName, this.manifestReader.Object);

            // Act
            var versions = archive.Versions;

            // Assert
            Assert.Contains(version1, versions);
            Assert.Contains(version2, versions);
        }
		
        public void Dispose()
        {
            if (this.testDirectory != null)
            {
                this.testDirectory.Delete(true);
            }
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
