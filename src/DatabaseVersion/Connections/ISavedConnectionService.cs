namespace dbversion.Connections
{
    using System.Collections.Generic;

    using dbversion.Settings;

    /// <summary>
    /// Used to manage the saved connections.
    /// </summary>
    public interface ISavedConnectionService
    {
        /// <summary>
        /// Gets the saved connections.
        /// </summary>
        /// <value>
        /// The saved connections.
        /// </value>
        IEnumerable<SavedConnection> SavedConnections
        {
            get;
        }

        /// <summary>
        /// Gets the default connection.
        /// </summary>
        /// <value>
        /// The default connection.
        /// </value>
        SavedConnection DefaultConnection
        {
            get;
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
        SavedConnection CreateSavedConnection(
            string name, string connectionString, string provider, string driverClass, string dialect);

        /// <summary>
        /// Saves the connections to settings.
        /// </summary>
        void SaveConnections();

        /// <summary>
        /// Loads the connections from settings.
        /// </summary>
        void LoadConnections();

        /// <summary>
        /// Deletes the connection.
        /// </summary>
        /// <param name='name'>
        /// The connection name.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection was deleted; otherwise <c>false</c>.
        /// </returns>
        bool DeleteConnection(string name);

        /// <summary>
        /// Sets the specified connection as the default.
        /// </summary>
        /// <param name='name'>
        /// The name of the connection to set as default.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection exists; otherwise <c>false</c>.
        /// </returns>
        bool SetDefaultConnection(string name);
    }
}

