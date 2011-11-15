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
    }
}

