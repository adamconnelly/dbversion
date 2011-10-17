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
    using dbversion.Tasks;
    using dbversion.Version;
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
        public void ShouldUseVersionProviderToGetHistory()
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

        [Fact]
        public void ShouldBeAbleToShowTasks()
        {
            // Arrange
            var command = this.CreateCommand();

            var tasks = CreateTasks();

            var versions = new[]
            {
                new ClassicVersion("1.23")
                {
                    CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53),
                    Tasks = tasks
                },
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());

            // Act
            command.Execute(new[] { "history", "-t" });

            // Assert
            Assert.Equal(CreateExpectedOutput(versions, HistoryOrder.Descending, true), messageService.Contents);
        }

        [Fact]
        public void ShouldNotShowTasksWhenShowTasksIsNotSpecified()
        {
            // Arrange
            var command = this.CreateCommand();

            var tasks = CreateTasks();

            var versions = new[]
            {
                new ClassicVersion("1.23")
                {
                    CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53),
                    Tasks = tasks
                },
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());

            // Act
            command.Execute(new[] { "history" });

            // Assert
            Assert.Equal(CreateExpectedOutput(versions, HistoryOrder.Descending, false), messageService.Contents);
        }

        [Fact]
        public void ShouldBeAbleToSpecifyASpecificVersion()
        {
            // Arrange
            var command = this.CreateCommand();

            var outputVersion = new ClassicVersion("1.24")
            {
                CreatedOn = new DateTime(2011, 10, 14, 9, 14, 29), UpdatedOn = new DateTime(2011, 10, 14, 10, 00, 23)
            };

            var versions = new[]
            {
                new ClassicVersion("1.23") { CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53) },
                outputVersion,
                new ClassicVersion("1.24.1") { CreatedOn = new DateTime(2011, 10, 16, 21, 59, 53), UpdatedOn = new DateTime(2011, 10, 16, 21, 59, 53) }
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());
            versionProvider.Setup(v => v.CreateVersion("1.24")).Returns(outputVersion);

            // Act
            command.Execute(new[] { "history", "-v", "1.24" });

            // Assert
            Assert.Equal(CreateExpectedOutput(new[] { outputVersion }), messageService.Contents);
        }

        [Fact]
        public void ShouldOutputAnErrorMessageIfTheSpecifiedVersionIsNotFound()
        {
            // Arrange
            var command = this.CreateCommand();

            var versions = new[]
            {
                new ClassicVersion("1.23") { CreatedOn = new DateTime(2011, 10, 13, 11, 12, 53), UpdatedOn = new DateTime(2011, 10, 13, 11, 12, 53) },
                new ClassicVersion("1.24.1") { CreatedOn = new DateTime(2011, 10, 16, 21, 59, 53), UpdatedOn = new DateTime(2011, 10, 16, 21, 59, 53) }
            };

            versionProvider.Setup(v => v.GetAllVersions(It.IsAny<ISession>())).Returns(versions);
            versionProvider.Setup(v => v.GetComparer()).Returns(new ClassicVersionProvider.ClassicVersionComparer());
            versionProvider.Setup(v => v.CreateVersion("1.24")).Returns(new ClassicVersion("1.24"));

            // Act
            command.Execute(new[] { "history", "-v", "1.24" });

            // Assert
            Assert.Equal("The specified version, 1.24, was not found." + Environment.NewLine, messageService.Contents);
        }

        private HistoryCommand CreateCommand()
        {
            var command = new HistoryCommand();

            command.VersionProvider = this.versionProvider.Object;
            command.MessageService = this.messageService;
            command.SessionFactoryProvider = this.sessionFactoryProvider.Object;
            command.SettingsService = this.settingsService.Object;
            command.PropertyService = this.propertyService.Object;

            return command;
        }

        private static IList<Task> CreateTasks ()
        {
            var task1 = new Mock<Task>();
            var task2 = new Mock<Task>();

            task1.Setup(t => t.Name).Returns("/path/to/mytask.sql");
            task1.Setup(t => t.UpdatedOnLocal).Returns(new DateTime(2011, 1, 1, 10, 59, 23));
            task1.Setup(t => t.ExecutionOrder).Returns(1);

            task2.Setup(t => t.Name).Returns("/path/to/another/task.sql");
            task2.Setup(t => t.UpdatedOnLocal).Returns(new DateTime(2011, 1, 1, 11, 00, 46));
            task2.Setup(t => t.ExecutionOrder).Returns(2);

            return new[] { task1.Object, task2.Object };
        }

        private static string CreateExpectedOutput(IEnumerable<ClassicVersion> versions, HistoryOrder sortOrder = HistoryOrder.Descending, bool showTasks = false)
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

                if (showTasks)
                {
                    IEnumerable<Task> sortedTasks;
                    if (sortOrder == HistoryOrder.asc || sortOrder == HistoryOrder.Ascending)
                    {
                        sortedTasks = version.Tasks.OrderBy(t => t.ExecutionOrder);
                    }
                    else
                    {
                        sortedTasks = version.Tasks.OrderByDescending(t => t.ExecutionOrder);
                    }

                    foreach (var task in sortedTasks)
                    {
                        builder.AppendLine(
                            string.Format(
                                "  {0} - {1}",
                                task.Name,
                                task.UpdatedOnLocal));
                    }
                }
            }

            return builder.ToString();
        }
    }

}

