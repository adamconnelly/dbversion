namespace dbversion.Console.Command
{
    /// <summary>
    /// A command that can be executed via the console.
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// Gets the name of the command. This is the name that will be used
        /// to execute the command at the command line.
        /// </summary>
        /// <value>
        /// The name of the command to execute.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets a short description of what the command does.
        /// </summary>
        /// <value>
        /// The description of the command.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Executes the command with the specified arguments.
        /// </summary>
        /// <param name='arguments'>
        /// The command line arguments.
        /// </param>
        void Execute(string[] arguments);
    }
}
