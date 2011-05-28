namespace DatabaseVersion.Connections
{
    using System.Data;

    /// <summary>
    /// Creates database connections.
    /// </summary>
    public interface IDbConnectionCreator
    {
        /// <summary>
        /// Indicates whether the creator can create a connection based on the supplied
        /// connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>True if a connection can be created, false otherwise.</returns>
        bool CanCreate(string connectionString);

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
