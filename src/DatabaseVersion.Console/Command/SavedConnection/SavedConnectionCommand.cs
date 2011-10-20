namespace dbversion.Console.Command.SavedConnection
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Connections;

    using dbversion.Console.Command;

    [Export(typeof(IConsoleCommand))]
    public class SavedConnectionCommand : IConsoleCommand
    {
        /// <summary>
        /// Gets the name of the command. This is the name that will be used
        /// to execute the command at the command line.
        /// </summary>
        /// <value>
        /// The name of the command to execute.
        /// </value>
        public string Name
        {
            get
            {
                return "saved-connection";
            }
        }

        /// <summary>
        /// Gets a short description of what the command does.
        /// </summary>
        /// <value>
        /// The description of the command.
        /// </value>
        public string Description
        {
            get
            {
                return "Allows connection details to be saved and managed.";
            }
        }

        /// <summary>
        /// Gets the command usage.
        /// </summary>
        /// <value>
        /// The command usage.
        /// </value>
        public string Usage
        {
            get
            {
                return "dbversion saved-connection [options]";
            }
        }

        /// <summary>
        /// Gets the parameters that can be used with the command.
        /// </summary>
        /// <value>
        /// The parameters or an empty enumerable.
        /// </value>
        public System.Collections.Generic.IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-n", "--name", "The name of the connection."),
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect.")
                };
            }
        }

        [Import]
        public IMessageService MessageService
        {
            get;
            set;
        }

        [Import]
        public ISavedConnectionService SavedConnectionService
        {
            get;
            set;
        }

        public void Execute (string[] args)
        {
            var arguments = ParseArguments(args);

            this.SavedConnectionService.LoadConnections();

            if (!string.IsNullOrEmpty(arguments.Name) &&
                !string.IsNullOrEmpty(arguments.ConnectionProvider) &&
                !string.IsNullOrEmpty(arguments.ConnectionString) &&
                !string.IsNullOrEmpty(arguments.Dialect) &&
                !string.IsNullOrEmpty(arguments.DriverClass))
            {
                var connection = this.SavedConnectionService.CreateSavedConnection(
                    arguments.Name,
                    arguments.ConnectionString,
                    arguments.ConnectionProvider,
                    arguments.DriverClass,
                    arguments.Dialect);
                this.SavedConnectionService.SaveConnections();

                this.MessageService.WriteLine(
                    string.Format("Created new saved connection \"{0}\":", connection.Name));
                this.WriteConnectionInfo(connection);
            }
            else if (!string.IsNullOrEmpty(arguments.Name) && arguments.SetDefaultConnection)
            {
                if (this.SavedConnectionService.SetDefaultConnection(arguments.Name))
                {
                    this.SavedConnectionService.SaveConnections();
                    this.MessageService.WriteLine(
                        string.Format("\"{0}\" is now the default connection.", arguments.Name));
                }
                else
                {
                    this.MessageService.WriteLine(
                        string.Format("There is no saved connection called \"{0}\".", arguments.Name));
                }
            }
            else if (!string.IsNullOrEmpty(arguments.Name) && arguments.Delete)
            {
                if (this.SavedConnectionService.DeleteConnection(arguments.Name))
                {
                    this.SavedConnectionService.SaveConnections();
                    this.MessageService.WriteLine(
                       string.Format("\"{0}\" has been deleted.", arguments.Name));
                }
                else
                {
                    this.MessageService.WriteLine(
                        string.Format("There is no saved connection called \"{0}\".", arguments.Name));
                }
            }
            else
            {
                var connections = this.GetConnections(arguments);

                if (connections.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(arguments.Name))
                    {
                        this.MessageService.WriteLine(
                            string.Format("There is no saved connection called \"{0}\".", arguments.Name));
                    }
                    else
                    {
                        this.MessageService.WriteLine("There are no saved connections.");
                    }
                }
                else
                {
                    foreach (var connection in connections)
                    {
                        this.MessageService.WriteLine(connection.Name + ":");
                        this.WriteConnectionInfo(connection);
                    }
                }
            }
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name='args'>
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The parsed arguments.
        /// </returns>
        private static SavedConnectionArguments ParseArguments(string[] args)
        {
            var arguments = new SavedConnectionArguments();
            var parserSettings = new CommandLineParserSettings();
            parserSettings.CaseSensitive = true;
            parserSettings.HelpWriter = System.Console.Out;

            var parser = new CommandLineParser(parserSettings);
            parser.ParseArguments(args, arguments);

            return arguments;
        }

        private IEnumerable<SavedConnection> GetConnections(SavedConnectionArguments arguments)
        {
            if (!string.IsNullOrEmpty(arguments.Name))
            {
                return this.SavedConnectionService.SavedConnections
                    .Where(c => c.Name == arguments.Name);
            }

            return this.SavedConnectionService.SavedConnections
                .Where(c => c.IsDefault)
                    .Union(
                        this.SavedConnectionService.SavedConnections
                            .Where(c => !c.IsDefault)
                            .OrderBy(c => c.Name));
        }

        private void WriteConnectionInfo(SavedConnection connection)
        {
            this.MessageService.WriteLine(string.Format("  Connection String: {0}", connection.ConnectionString));
            this.MessageService.WriteLine(string.Format("  Driver Class: {0}", connection.DriverClass));
            this.MessageService.WriteLine(string.Format("  Provider: {0}", connection.ConnectionProvider));
            this.MessageService.WriteLine(string.Format("  Dialect: {0}", connection.Dialect));
            this.MessageService.WriteLine(string.Format("  Default: {0}", connection.IsDefault));
            this.MessageService.WriteLine();
        }
    }
}

