namespace DatabaseVersion.Connections
{
    using System;

    /// <summary>
    /// Thrown if the connection string could not be used to create a connection.
    /// </summary>
    public class UnsupportedConnectionStringException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedConnectionStringException"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string that is not supported.</param>
        public UnsupportedConnectionStringException(string connectionString)
            : base(string.Format("No connection creator was found that supports the specified connection string: {0}", connectionString))
        {
        }
    }
}
