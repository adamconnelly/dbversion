namespace dbversion.Console.Command.History
{
    using CommandLine;

    /// <summary>
    /// The arguments for the history command.
    /// </summary>
    public class HistoryArguments : ConnectionArguments
    {
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

        [Option("v", "version")]
        public string Version;
    }
}

