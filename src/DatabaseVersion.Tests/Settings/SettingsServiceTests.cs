namespace dbversion.Tests.Settings
{
    using System;
    using System.Linq;
    using System.IO;

    using dbversion.Property;
    using dbversion.Settings;
    using dbversion.Utils;

    using Xunit;

    public class SettingsServiceTests
    {
        [Fact]
        public void ShouldReturnCorrectSettingsDirectory()
        {
            // Arrange
            var expected = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SettingsService.DirectoryName);
            var settingsService = new SettingsService();

            // Act
            var settingsDirectory = settingsService.SettingsDirectory;

            // Assert
            Assert.Equal(expected, settingsDirectory);
        }

        #region Write
        [Fact]
        public void ShouldBeAbleToWriteSettingsFile()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("This is a test");
                    writer.Flush();

                    service.Write(stream, "mySettings.txt");
                }
            }

            // Assert
            using (var reader = new StreamReader(Path.Combine(service.SettingsDirectory, "mySettings.txt")))
            {
                Assert.Equal("This is a test", reader.ReadToEnd());
            }
        }

        public void ShouldThrowExceptionIfTryingToWriteANullStream()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Write(null, "mySettings.xml"));

            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfTryingToWriteToANullFileName()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Write(new MemoryStream(), null));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfTryingToWriteToAnEmptyFileName()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Write(new MemoryStream(), string.Empty));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        #endregion

        #region Serialize
        [Fact]
        public void ShouldBeAbleToSerializeAnObject()
        {
            // Arrange
            var service = new SettingsService();

            var properties = new PropertyCollection();
            properties.Properties.Add(new Property{ Key = "property.one", Value = "valueOne" });
            properties.Properties.Add(new Property{ Key = "property.two", Value = "valueTwo" });

            string expected;

            using (var reader = new StreamReader(XmlSerializer.Serialize(properties)))
            {
                expected = reader.ReadToEnd();
            }

            // Act
            service.Serialize(properties, "test.properties.xml");

            // Assert
            using (var reader = new StreamReader(Path.Combine(service.SettingsDirectory, "test.properties.xml")))
            {
                Assert.Equal(expected, reader.ReadToEnd());
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfObjectToSerializeIsNull()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Serialize(null, "myFile.txt"));

            // Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfFileNameToSerializeToIsNull()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Serialize(new object(), null));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ShouldThrowExceptionIfFileNameToSerializeToIsEmpty()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var exception = Record.Exception(() => service.Serialize(new object(), string.Empty));

            // Assert
            Assert.IsType<ArgumentException>(exception);
        }

        #endregion

        [Fact]
        public void ShouldBeAbleToDeserialiseAnObject()
        {
            // Arrange
            var service = new SettingsService();

            var properties = new PropertyCollection();
            properties.Properties.Add(new Property{ Key = "property.one", Value = "valueOne" });
            properties.Properties.Add(new Property{ Key = "property.two", Value = "valueTwo" });

            using (var stream = XmlSerializer.Serialize(properties))
            {
                using (var fileStream = new FileStream(Path.Combine(service.SettingsDirectory, "test.serialized.properties.xml"), FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            // Act
            var deserializedProperties = service.DeSerialize<PropertyCollection>("test.serialized.properties.xml");

            // Assert
            Assert.Equal(2, deserializedProperties.Properties.Count);
            Assert.Equal("valueOne", deserializedProperties.Properties.Single(p => p.Key == "property.one").Value);
            Assert.Equal("valueTwo", deserializedProperties.Properties.Single(p => p.Key == "property.two").Value);
        }

        [Fact]
        public void ShouldReturnDefaultTypeIfSettingsFileToDeserializeDoesNotExist()
        {
            // Arrange
            var service = new SettingsService();

            // Act
            var result1 = service.DeSerialize<PropertyCollection>("non-existent-settings.file1");
            var result2 = service.DeSerialize<int>("non-existent-settings.file2");

            // Assert
            Assert.Null(result1);
            Assert.Equal(default(int), result2);
        }
    }
}
