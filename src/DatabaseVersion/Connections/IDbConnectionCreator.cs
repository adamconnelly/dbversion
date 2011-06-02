namespace DatabaseVersion.Connections
{
    using System.Data;

    /// <summary>
    /// Creates database connections.
    /// </summary>
    public interface IDbConnectionCreator
    {
        /// <summary>
        /// Gets the name of the connection type that this creator supports.
        /// </summary>
        string ConnectionType { get; }

        /// <summary>
        /// Creates a new database connection from the supplied connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to create the connection using.
        /// </param>
        /// <returns>The database connection.</returns>
        /// <exception cref="UnsupportedConnectionStringException">
        /// Thrown if the supplied connection string is not supported by this creator.
        /// </exception>
        IDbConnection Create(string connectionString);
    }
}
