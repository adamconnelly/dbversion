namespace DatabaseVersion.Connections
{
    using System.Data;

    /// <summary>
    /// A factory for creating database connections.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates a new connection based on the supplied connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="connectionType">The type of connection to create.</param>
        /// <returns>The new connection.</returns>
        /// <exception cref="UnsupportedConnectionStringException">
        /// Thrown if no <see cref="IDbConnectionCreator"/> could be found that supports
        /// the supplied connection string.
        /// </exception>
        IDbConnection Create(string connectionString, string connectionType);
    }
}
