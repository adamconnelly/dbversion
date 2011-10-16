namespace dbversion.Console.Command.History
{
    using CommandLine;

    /// <summary>
    /// The arguments for the history command.
    /// </summary>
    public class HistoryArguments
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        [Option("c", "connectionString")]
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

        /// <summary>
        /// The sort order.
        /// </summary>
        [Option("o", "order")]
        public HistoryOrder SortOrder = HistoryOrder.Descending;

        /// <summary>
        /// Indicates whether the tasks for each version should also be output.
        /// </summary>
        [Option("t", "showTasks")]
        public bool ShowTasks;
    }
}

