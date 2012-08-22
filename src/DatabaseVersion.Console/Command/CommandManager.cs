namespace dbversion.Console.Command
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    /// <summary>
    /// Used to route a set of arguments to the correct command.
    /// </summary>
    [Export(typeof(ICommandManager))]
    public class CommandManager : ICommandManager
    {
        /// <summary>
        /// Gets or sets the available commands.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        [ImportMany]
        public IEnumerable<IConsoleCommand> Commands
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message service.
        /// </summary>
        /// <value>
        /// The message service.
        /// </value>
        [Import]
        public IMessageService MessageService
        {
            get;
            set;
        }

        /// <summary>
        /// Execute the command specified in the arguments.
        /// </summary>
        /// <param name='args'>
        /// The command to execute.
        /// </param>
        /// <returns>Returns the result of Executing the Command</returns>
        public bool Execute(string[] args)
        {
            bool result = false;
            if (args == null || args.Length == 0)
            {
                this.MessageService.WriteLine("Usage: dbversion command [Options]");
            }
            else
            {
                var command = this.Commands.FirstOrDefault(c => c.Name == args[0]);
                if (command == null)
                {
                    this.MessageService.WriteLine(string.Format("Unknown command '{0}'.", args[0]));
                }
                else
                {
                    result = command.Execute(args);
                }
            }
            return result;
        }
    }
}

