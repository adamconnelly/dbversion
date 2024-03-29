using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using dbversion.Archives.File;
using System.IO;
using dbversion.Archives;
using dbversion.Tests.Utils;

namespace dbversion.Tests.Archives.File
{
    public class FileDatabaseArchiveFactoryTests
    {
        #region CanCreate
        [Fact]
        public void ShouldBeAbleToCreateAnArchiveFromADirectory()
        {
            // Arrange
            FileDatabaseArchiveFactory factory = new FileDatabaseArchiveFactory();
            DirectoryInfo testDirectory = FileUtil.CreateTempDirectory();

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
            DirectoryInfo testDirectory = FileUtil.CreateTempDirectory();

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
            DirectoryInfo testDirectory = FileUtil.CreateTempDirectory();

            // Act
            IDatabaseArchive archive = factory.Create(testDirectory.FullName);

            // Assert
            Assert.Equal(testDirectory.FullName, archive.ArchivePath);

            testDirectory.Delete();
        }
        #endregion
    }
}
