namespace dbversion.Console.Command.History
{
    /// <summary>
    /// Used to specify the order that the output of the history command should be displayed.
    /// </summary>
    public enum HistoryOrder
    {
        /// <summary>
        /// The history should be sorted by version in ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// The history should be sorted by version in ascending order.
        /// </summary>
        asc,

        /// <summary>
        /// The history should be sorted by version in descending order.
        /// </summary>
        Descending,

        /// <summary>
        /// The history should be sorted by version in descending order.
        /// </summary>
        desc
    }
}

