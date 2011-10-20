namespace dbversion.Connections
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using dbversion.Settings;

    /// <summary>
    /// Used to manage the saved connections.
    /// </summary>
    [Export(typeof(ISavedConnectionService))]
    public class SavedConnectionService : ISavedConnectionService
    {
        /// <summary>
        /// The saved connections.
        /// </summary>
        private List<SavedConnection> savedConnections = new List<SavedConnection>();

        /// <summary>
        /// Gets or sets the settings service.
        /// </summary>
        /// <value>
        /// The settings service.
        /// </value>
        [Import]
        public ISettingsService SettingsService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the saved connections.
        /// </summary>
        /// <value>
        /// The saved connections.
        /// </value>
        public IEnumerable<SavedConnection> SavedConnections
        {
            get
            {
                return this.savedConnections;
            }
        }

        /// <summary>
        /// Creates a new saved connection or updates an existing saved connection if a connection
        /// with the specified name already exists.
        /// </summary>
        /// <param name='name'>
        /// The name of the saved connection.
        /// </param>
        /// <param name='connectionString'>
        /// The connection string.
        /// </param>
        /// <param name='provider'>
        /// The connection provider.
        /// </param>
        /// <param name='driverClass'>
        /// The driver class.
        /// </param>
        /// <param name='dialect'>
        /// The dialect.
        /// </param>
        /// <returns>
        /// The saved connection.
        /// </returns>
        public SavedConnection CreateSavedConnection(
            string name, string connectionString, string provider, string driverClass, string dialect)
        {
            var isDefault = this.savedConnections.Count == 0;

            var connection = this.SavedConnections.FirstOrDefault(c => c.Name == name);
            if (connection != null)
            {
                connection.ConnectionString = connectionString;
                connection.ConnectionProvider = provider;
                connection.DriverClass = driverClass;
                connection.Dialect = dialect;
            }
            else
            {
                connection = new SavedConnection(name, connectionString, provider, driverClass, dialect, isDefault);
                this.savedConnections.Add(connection);
            }

            return connection;
        }

        /// <summary>
        /// Saves the connections to settings.
        /// </summary>
        public void SaveConnections()
        {
            this.SettingsService.Serialize(savedConnections, "saved-connections.xml");
        }

        /// <summary>
        /// Loads the connections from settings.
        /// </summary>
        public void LoadConnections()
        {
            var connections = this.SettingsService.DeSerialize<List<SavedConnection>>("saved-connections.xml");
            if (connections != null)
            {
                this.savedConnections = connections;
            }
            else
            {
                this.savedConnections = new List<SavedConnection>();
            }
        }

        /// <summary>
        /// Deletes the connection.
        /// </summary>
        /// <param name='name'>
        /// The name of the connection to delete.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection was deleted; otherwise <c>false</c>.
        /// </returns>
        public bool DeleteConnection(string name)
        {
            var connection = this.savedConnections.FirstOrDefault(c => c.Name == name);
            if (connection != null)
            {
                return this.savedConnections.Remove(connection);
            }

            return false;
        }

        /// <summary>
        /// Sets the specified connection as the default.
        /// </summary>
        /// <param name='name'>
        /// The name of the connection to set as default.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection exists; otherwise <c>false</c>.
        /// </returns>
        public bool SetDefaultConnection(string name)
        {
            var connection = this.savedConnections.FirstOrDefault(c => c.Name == name);
            if (connection != null)
            {
                this.savedConnections.ForEach(c => c.IsDefault = false);
                connection.IsDefault = true;
                return true;
            }

            return false;
        }
    }
}

