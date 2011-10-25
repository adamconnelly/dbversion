namespace DatabaseVersion.Tests.Connections
{
    using System.Collections.Generic;
    using System.Linq;

    using dbversion.Connections;
    using dbversion.Settings;

    using Moq;

    using Xunit;

    public class SavedConnectionServiceTests
    {
        #region Default Connection
        [Fact]
        public void ShouldMakeFirstSavedConnectionTheDefault()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection(
                "library", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.True(connection.IsDefault);
        }

        [Fact]
        public void ShouldNotMakeOtherConnectionsTheDefault()
        {
            // Arrange
            var service = new SavedConnectionService();

            service.CreateSavedConnection("a", "conn", "provider", "driverClass", "dialect");

            // Act
            var connection = service.CreateSavedConnection("b", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.False(connection.IsDefault);
        }
        #endregion

        #region Connection Properties
        [Fact]
        public void ShouldSetConnectionNameWhenCreatingConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal("connection1", connection.Name);
        }

        [Fact]
        public void ShouldSetConnectionStringWhenCreatingConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal("conn", connection.ConnectionString);
        }

        [Fact]
        public void ShouldSetConnectionProviderWhenCreatingConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal("provider", connection.ConnectionProvider);
        }

        [Fact]
        public void ShouldSetDriverClassWhenCreatingConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal("driverClass", connection.DriverClass);
        }

        [Fact]
        public void ShouldSetDialectWhenCreatingConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal("dialect", connection.Dialect);
        }
        #endregion

        [Fact]
        public void ShouldReplaceConnectionIfCreatingConnectionWithSameName()
        {
            // Arrange
            var service = new SavedConnectionService();

            // Act
            service.CreateSavedConnection("connection1", null, null, null, null);
            var connection = service.CreateSavedConnection("connection1", "conn", "provider", "driverClass", "dialect");

            // Assert
            Assert.Equal(1, service.SavedConnections.Count());
            Assert.Same(connection, service.SavedConnections.Single());
        }

        #region Save
        [Fact]
        public void ShouldBeAbleToSaveConnections()
        {
            // Arrange
            var service = new SavedConnectionService();
            var settingsService = new Mock<ISettingsService>();

            service.SettingsService = settingsService.Object;

            List<SavedConnection> expectedConnections = new List<SavedConnection>();
            expectedConnections.Add(service.CreateSavedConnection("a", null, null, null, null));
            expectedConnections.Add(service.CreateSavedConnection("b", null, null, null, null));

            // Act
            service.SaveConnections();

            // Assert
            settingsService.Verify(s => s.Serialize(expectedConnections, "saved-connections.xml"));
        }
        #endregion

        #region Load
        [Fact]
        public void ShouldBeAbleToLoadSavedConnections()
        {
            // Arrange
            var service = new SavedConnectionService();
            var settingsService = new Mock<ISettingsService>();

            service.SettingsService = settingsService.Object;

            List<SavedConnection> expectedConnections = new List<SavedConnection>();
            expectedConnections.Add(new SavedConnection("a", null, null, null, null, true));
            expectedConnections.Add(new SavedConnection("b", null, null, null, null, false));

            settingsService.Setup(
                s => s.DeSerialize<List<SavedConnection>>("saved-connections.xml")).Returns(expectedConnections);

            // Act
            service.LoadConnections();

            // Assert
            Assert.Equal(expectedConnections, service.SavedConnections);
        }

        [Fact]
        public void ShouldHaveNoConnectionsIfNoConnectionsAreSavedInSettings()
        {
            // Arrange
            var service = new SavedConnectionService();
            var settingsService = new Mock<ISettingsService>();

            service.SettingsService = settingsService.Object;

            settingsService.Setup(
                s => s.DeSerialize<List<SavedConnection>>("saved-connections.xml")).Returns((List<SavedConnection>)null);

            // Act
            service.LoadConnections();

            // Assert
            Assert.Empty(service.SavedConnections);
        }
        #endregion

        [Fact]
        public void ShouldBeAbleToDeleteASavedConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            service.CreateSavedConnection("a", null, null, null, null);
            service.CreateSavedConnection("b", null, null, null, null);
            service.CreateSavedConnection("c", null, null, null, null);

            // Act
            service.DeleteConnection("b");

            // Assert
            Assert.Equal(2, service.SavedConnections.Count());
            Assert.Null(service.SavedConnections.SingleOrDefault(c => c.Name == "b"));
        }

        #region Default

        [Fact]
        public void ShouldBeAbleToSetConnectionAsDefault()
        {
            // Arrange
            var service = new SavedConnectionService();

            var connectionA = service.CreateSavedConnection("a", null, null, null, null);
            var connectionB = service.CreateSavedConnection("b", null, null, null, null);
            var connectionC = service.CreateSavedConnection("c", null, null, null, null);

            // Act
            var success = service.SetDefaultConnection("b");

            // Assert
            Assert.True(connectionB.IsDefault, "Connection B should be the default");
            Assert.False(connectionA.IsDefault, "Connection A should not be default");
            Assert.False(connectionC.IsDefault, "Connection C should not be default");

            Assert.True(success, "The method should have succeeded");
        }

        [Fact]
        public void ShouldReturnFalseIfConnectionToDefaultDoesNotExist()
        {
            // Arrange
            var service = new SavedConnectionService();

            var connectionA = service.CreateSavedConnection("a", null, null, null, null);
            service.CreateSavedConnection("b", null, null, null, null);
            service.CreateSavedConnection("c", null, null, null, null);

            // Act
            var success = service.SetDefaultConnection("ZZZ");

            // Assert
            Assert.True(connectionA.IsDefault, "Connection A should still be default");

            Assert.False(success, "The method should not have succeeded");
        }

        #endregion

        [Fact]
        public void ShouldBeAbleToAddANewConnection()
        {
            // Arrange
            var service = new SavedConnectionService();

            var connection = new SavedConnection("connection", "connString", null, null, null, false);

            // Act
            service.AddConnection(connection);

            // Assert
            Assert.Same(connection, service.SavedConnections.Single(c => c.Name == "connection"));
        }

        [Fact]
        public void ShouldNotAddAConnectionAsDefault()
        {
            // Arrange
            var service = new SavedConnectionService();

            var connection = new SavedConnection("connection", "connString", null, null, null, true);

            // Act
            service.AddConnection(connection);

            // Assert
            Assert.False(service.SavedConnections.Single(c => c.Name == "connection").IsDefault);
        }
    }
}

