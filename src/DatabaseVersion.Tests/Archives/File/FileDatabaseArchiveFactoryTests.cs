using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using DatabaseVersion.Archives.File;
using System.IO;
using DatabaseVersion.Archives;

namespace DatabaseVersion.Tests.Archives.File
{
    public class FileDatabaseArchiveFactoryTests
    {
        #region CanCreate
        [Fact]
        public void ShouldBeAbleToCreateAnArchiveFromADirectory()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();
            DirectoryInfo testDirectory = this.CreateTempDirectory();

            // Act
            bool canCreate = factory.CanCreate(testDirectory.FullName);

            // Assert
            Assert.True(canCreate);

            testDirectory.Delete(true);
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveFromAFile()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();
            string tempFile = Path.GetTempFileName();

            // Act
            bool canCreate = factory.CanCreate(tempFile);

            // Assert
            Assert.False(canCreate);

            new FileInfo(tempFile).Delete();
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveWhenPathIsNull()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();

            // Act
            bool canCreate = factory.CanCreate(null);

            // Assert
            Assert.False(canCreate);
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveWhenPathIsEmpty()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();

            // Act
            bool canCreate = factory.CanCreate(null);

            // Assert
            Assert.False(canCreate);
        }
        #endregion

        #region Create
        [Fact]
        public void ShouldCreateAFileDatabaseArchive()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();
            DirectoryInfo testDirectory = CreateTempDirectory();

            // Act
            IDatabaseArchive archive = factory.Create(testDirectory.FullName);

            // Assert
            Assert.IsType<FileDatabaseArchive>(archive);
        }

        [Fact]
        public void ShouldThrowExceptionIfPathToCreateIsNull()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();

            // Act
            Exception exception = Record.Exception(() => factory.Create(null));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfPathToCreateIsEmpty()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();

            // Act
            Exception exception = Record.Exception(() => factory.Create(string.Empty));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ShouldCreateArchiveWithTheSpecifiedPath()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();
            DirectoryInfo testDirectory = CreateTempDirectory();

            // Act
            IDatabaseArchive archive = factory.Create(testDirectory.FullName);

            // Assert
            Assert.Equal(testDirectory.FullName, archive.ArchivePath);

            testDirectory.Delete();
        }
        #endregion

        private DirectoryInfo CreateTempDirectory()
        {
            DirectoryInfo info = null;
            string tempDirectory = Path.GetTempPath();

            do
            {
                string directoryName = Path.GetRandomFileName();

                info = new DirectoryInfo(Path.Combine(tempDirectory, directoryName));
            }
            while (info.Exists);

            info.Create();

            return info;
        }
    }
}
