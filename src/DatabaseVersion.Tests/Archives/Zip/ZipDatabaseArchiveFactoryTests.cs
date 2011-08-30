using Xunit;
using Ionic.Zip;
using dbversion.Archives.Zip;
using System.IO;
using System;
using System.Collections.Generic;
using dbversion.Archives;
using dbversion.Manifests;
using Moq;
namespace dbversion.Tests.Archives.Zip
{
    public class ZipDatabaseArchiveFactoryTests : IDisposable
    {
        private readonly List<string> tempFiles = new List<string>();

        #region CanCreate
        [Fact]
        public void ShouldBeAbleToCreateAnArchiveFromAZipFile()
        {
            using (ZipFile zipFile = new ZipFile())
            {
                // Arrange
                string fileName = this.CreateTempFile();
                zipFile.Save(fileName);

                ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

                // Act
                bool canCreate = factory.CanCreate(fileName);

                // Assert
                Assert.True(canCreate);
            }
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveFromAnotherTypeOfFile()
        {
            // Arrange
            string fileName = this.CreateTempFile();
            ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

            // Act
            bool canCreate = factory.CanCreate(fileName);

            // Assert
            Assert.False(canCreate);
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveFromANullPath()
        {
            // Arrange
            ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

            // Act
            bool canCreate = factory.CanCreate(null);

            // Assert
            Assert.False(canCreate);
        }

        [Fact]
        public void ShouldNotBeAbleToCreateAnArchiveFromAnEmptyPath()
        {
            // Arrange
            ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

            // Act
            bool canCreate = factory.CanCreate(string.Empty);

            // Assert
            Assert.False(canCreate);
        }
        #endregion

        #region Create
        [Fact]
        public void ShouldCreateAZipDatabaseArchive()
        {
            using (ZipFile zipFile = new ZipFile())
            {
                // Arrange
                ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();
                factory.ManifestReader = new Mock<IManifestReader>().Object;
                string fileName = this.CreateTempFile();
                zipFile.Save(fileName);

                // Act
                IDatabaseArchive archive = factory.Create(fileName);

                // Assert
                Assert.IsType<ZipDatabaseArchive>(archive);
            }
        }

        [Fact]
        public void ShouldCreateAnArchiveWithTheCorrectArchivePath()
        {
            // Arrange
            using (ZipFile zipFile = new ZipFile())
            {
                ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();
                factory.ManifestReader = new Mock<IManifestReader>().Object;
                string fileName = this.CreateTempFile();
                zipFile.Save(fileName);

                // Act
                IDatabaseArchive archive = factory.Create(fileName);

                // Assert
                Assert.Equal(fileName, archive.ArchivePath);
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfFileNameToCreateArchiveFromIsNull()
        {
            // Arrange
            ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

            // Act
            Exception exception = Record.Exception(() => factory.Create(null));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfFileNameToCreateArchiveFromIsEmpty()
        {
            // Arrange
            ZipDatabaseArchiveFactory factory = new ZipDatabaseArchiveFactory();

            // Act
            Exception exception = Record.Exception(() => factory.Create(string.Empty));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (string file in this.tempFiles)
                {
                    System.IO.File.Delete(file);
                }
            }
        }

        private string CreateTempFile()
        {
            string tempFileName = Path.GetTempFileName();
            this.tempFiles.Add(tempFileName);

            return tempFileName;
        }
    }
}
