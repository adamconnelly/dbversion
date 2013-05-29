namespace dbversion.Console.Command.Create
{
    using CommandLine;

    /// <summary>
    /// The arguments for the create command.
    /// </summary>
    public class CreateArguments : ConnectionArguments
    {
        /// <summary>
        /// The version to create or upgrade to.
        /// </summary>
        [Option("v", "version", HelpText = "The version to create or upgrade to.")]
        public string Version;

        [Option(null, "simulate", HelpText = "Indicates that the update should be simulated and no actual changes should be made.")]
        public bool IsSimulatingUpdate;
    }
}

