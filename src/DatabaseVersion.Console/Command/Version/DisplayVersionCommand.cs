namespace dbversion.Console.Command.Version
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Displays the current application version.
    /// </summary>
    [Export(typeof(IConsoleCommand))]
    public class DisplayVersionCommand : IConsoleCommand
    {
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
        /// Gets the name of the command.
        /// </summary>
        /// <value>
        /// The name of the command.
        /// </value>
        public string Name
        {
            get { return "version"; }
        }

        public string Description
        {
            get { return "Displays version information and then exits."; }
        }

        public string Usage
        {
            get
            {
                return "dbversion " + this.Name;
            }
        }

        public IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return Enumerable.Empty<CommandParameter>();
            }
        }

        /// <summary>
        /// Execute the command using the specified args.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        /// <returns>Returns the result of Executing the Command</returns>
        public bool Execute(string[] args)
        {
            var assembly = typeof(DisplayVersionCommand).Assembly;
            var assemblyName = typeof(DisplayVersionCommand).Assembly.GetName();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.MessageService.WriteLine(string.Format("{0} {1}", assemblyName.Name, assemblyName.Version));
            this.MessageService.WriteLine(fileVersionInfo.LegalCopyright);
            this.MessageService.WriteLine("License MIT: The MIT License");
            this.MessageService.WriteLine("This is free software: you are free to change and redistribute it.");
            this.MessageService.WriteLine("There is NO WARRANTY, to the extent permitted by law.");

            return true;
        }
    }
}

