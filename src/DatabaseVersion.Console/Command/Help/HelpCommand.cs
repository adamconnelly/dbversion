namespace dbversion.Console.Command.Help
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    /// <summary>
    /// A command that can be used to get help about using the application.
    /// </summary>
    [Export(typeof(IConsoleCommand))]
    public class HelpCommand : IConsoleCommand
    {
        /// <summary>
        /// Gets or sets the console commands.
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
        /// Gets the name of the command. This is the name that will be used
        /// to execute the command at the command line.
        /// </summary>
        /// <value>
        /// The name of the command to execute.
        /// </value>
        public string Name
        {
            get { return "help"; }
        }

        /// <summary>
        /// Gets a short description of what the command does.
        /// </summary>
        /// <value>
        /// The description of the command.
        /// </value>
        public string Description
        {
            get { return "Displays information about using the application and then exits."; }
        }

        /// <summary>
        /// Execute the command with the specified args.
        /// </summary>
        /// <param name='args'>
        /// The arguments to the command.
        /// </param>
        public void Execute(string[] args)
        {
            var maxCommandLength = this.Commands.Max(c => c.Name.Length);

            foreach (var command in this.Commands.OrderBy(c => c.Name))
            {
                var padding = new string(' ', maxCommandLength - command.Name.Length + 2);
                this.MessageService.WriteLine(string.Format("  {0}{1}{2}", command.Name, padding, command.Description));
            }
        }
    }
}

