namespace dbversion.Console.Command
{
    /// <summary>
    /// Used to manage console command.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Executes the correct command depending on the arguments specified.
        /// </summary>
        /// <param name='args'>
        /// The command arguments.
        /// </param>
        /// <returns>Returns the result of Executing the Command</returns>
        bool Execute(string[] args);
    }
}

