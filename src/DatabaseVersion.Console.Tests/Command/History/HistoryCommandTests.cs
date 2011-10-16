namespace dbversion.Console.Tests.Command.History
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using dbversion.Console.Command.History;
    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;
    using dbversion.Version.ClassicVersion;

    using Moq;

    using NHibernate;

    using Xunit;

    public class HistoryCommandTests
    {
        private readonly Mock<IVersionProvider> versionProvider = new Mock<IVersionProvider>();
        private readonly MessageServiceMock messageService = new MessageServiceMock();
        private readonly Mock<ISessionFactoryProvider> sessionFactoryProvider = new Mock<ISessionFactoryProvider>() { DefaultValue = DefaultValue.Mock };
        private readonly Mock<ISettingsService> settingsService = new Mock<ISettingsService>();
        private readonly Mock<IPropertyService> propertyService = new Mock<IPropertyService>();

        [Fact]
        public void ShouldHaveCorrectCommandName()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("history", name);
        }

        [Fact]
        public void ShouldHaveCorrectDescription()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var description = command.Description;

            // Assert
            Assert.Equal("Prints out a history of the installed versions.", description);
        }

        [Fact]
        public void ShouldHaveCorrectCommandVersion()
        {
            // Arrange
            var command = new HistoryCommand();

            // Act
            var usage = command.Usage;

            // Assert
            Assert.Equal(string.Format("dbversion {0} [options]", command.Name), usage);
        }

        [Fact]
        public void ShouldUseVersionProviderToGetCurrentVersion()
        {
            // Arrange
            var command = this.CreateCommand();

            var versions = new[]
            {
                new ClassicVersion("1.23") { CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53) },
                new ClassicVersion("1.24") { CreatedOn = new DateTime(2011, 10, 14, 9, 14, 29), UpdatedOn = new DateTime(2011, 10, 14, 10, 00, 23) },
                new ClassicVersion("1.24.1") { CreatedOn = new DateTime(2011, 10, 16, 21, 59, 53), UpdatedOn = new DateTime(2011, 10, 16, 21, 59, 53) }
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());
            
            // Act
            command.Execute(new[] { "history" });

            // Assert
            Assert.Equal(CreateExpectedOutput(versions), messageService.Contents);
        }

        [Fact]
        public void ShouldBeAbleToSortVersionsInAscendingOrder()
        {
            // Arrange
            var command = this.CreateCommand();

            var versions = new[]
            {
                new ClassicVersion("1.23") { CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53) },
                new ClassicVersion("1.24") { CreatedOn = new DateTime(2011, 10, 14, 9, 14, 29), UpdatedOn = new DateTime(2011, 10, 14, 10, 00, 23) },
                new ClassicVersion("1.24.1") { CreatedOn = new DateTime(2011, 10, 16, 21, 59, 53), UpdatedOn = new DateTime(2011, 10, 16, 21, 59, 53) }
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());
            
            // Act
            command.Execute(new[] { "history", "-o", "asc" });

            // Assert
            Assert.Equal(CreateExpectedOutput(versions, HistoryOrder.Ascending), messageService.Contents);
        }

        private HistoryCommand CreateCommand ()
        {
            var command = new HistoryCommand();

            command.VersionProvider = this.versionProvider.Object;
            command.MessageService = this.messageService;
            command.SessionFactoryProvider = this.sessionFactoryProvider.Object;
            command.SettingsService = this.settingsService.Object;
            command.PropertyService = this.propertyService.Object;

            return command;
        }

        private static string CreateExpectedOutput(IEnumerable<ClassicVersion> versions, HistoryOrder sortOrder = HistoryOrder.Descending)
        {
            StringBuilder builder = new StringBuilder();

            IEnumerable<ClassicVersion> sortedVersions;
            if (sortOrder == HistoryOrder.asc || sortOrder == HistoryOrder.Ascending)
            {
                sortedVersions = versions.OrderBy(v => v.Version);
            }
            else
            {
                sortedVersions = versions.OrderByDescending(v => v.Version);
            }

            foreach (var version in sortedVersions)
            {
                builder.AppendLine(
                    string.Format(
                        "{0} Installed - {1}, Updated - {2}",
                        version.VersionText,
                        version.CreatedOnLocal,
                        version.UpdatedOnLocal));
            }

            return builder.ToString();
        }
    }

}

