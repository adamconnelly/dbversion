namespace dbversion.Console.Command.SavedConnection
{
    using System.ComponentModel;
    using System;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// The command line arguments that define how the application should function.
    /// </summary>
    public class SavedConnectionArguments
    {
        /// <summary>
        /// The archive containing the database scripts.
        /// </summary>
        [Option("n", "name")]
        public string Name;

        /// <summary>
        /// The connection string to the database.
        /// </summary>
        [Option("c", "connectionString", HelpText = "The database connection string.")]
        public string ConnectionString;
        
        /// <summary>
        /// Gets or sets the hibernate connection provider.
        /// </summary>
        [Option("p", "connectionProvider", HelpText = "The hibernate connection provider.")]
        public string ConnectionProvider;

        /// <summary>
        /// Gets or sets the hibernate driver class.
        /// </summary>
        [Option("d", "driverClass", HelpText = "The hibernate driver class.")]
        public string DriverClass;

        /// <summary>
        /// Gets or sets the hibernate dialect.
        /// </summary>
        [Option("l", "dialect", HelpText = "The hibernate dialect.")]
        public string Dialect;

        [Option("r", "delete")]
        public bool Delete;

        [Option(null, "default")]
        public bool SetDefaultConnection;
    }
}
