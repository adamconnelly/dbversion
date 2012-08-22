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
                    new CommandParameter("-l", "--dialect", "The hibernate dialect."),
                    new CommandParameter("-r", "--remove", "Deletes the connection specified using the -n option."),
                    new CommandParameter(string.Empty, "--default", "Sets the connection specified using the -n option as the default."),
                    new CommandParameter("-t", "--template", "Uses the specified connection as a template.")
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
        

        public bool Execute (string[] args)
        {
            var arguments = ParseArguments(args);

            this.SavedConnectionService.LoadConnections();

            if (!string.IsNullOrEmpty(arguments.Name) &&
                !string.IsNullOrEmpty(arguments.TemplateConnection))
            {
                this.CreateFromTemplate(arguments);
            }
            else if (!string.IsNullOrEmpty(arguments.Name) &&
                (!string.IsNullOrEmpty(arguments.ConnectionProvider) ||
                !string.IsNullOrEmpty(arguments.ConnectionString) ||
                !string.IsNullOrEmpty(arguments.Dialect) ||
                !string.IsNullOrEmpty(arguments.DriverClass)))
            {
                this.CreateNewConnection(arguments);
            }
            else if (!string.IsNullOrEmpty(arguments.Name) && arguments.SetDefaultConnection)
            {
                this.SetDefaultConnection(arguments);
            }
            else if (!string.IsNullOrEmpty(arguments.Name) && arguments.Delete)
            {
                this.DeleteConnection(arguments);
            }
            else
            {
                this.ListConnections(arguments);
            }
            return true;
        }

        private void CreateFromTemplate (SavedConnectionArguments arguments)
        {
            var template = this.SavedConnectionService.SavedConnections.FirstOrDefault(
             c => c.Name == arguments.TemplateConnection);
         
            if (template != null)
            {
                var connectionString = !string.IsNullOrEmpty(arguments.ConnectionString)
                    ? arguments.ConnectionString : template.ConnectionString;
                var driverClass = !string.IsNullOrEmpty(arguments.DriverClass)
                    ? arguments.DriverClass : template.DriverClass;
                var dialect = !string.IsNullOrEmpty(arguments.Dialect)
                    ? arguments.Dialect : template.Dialect;
                var provider = !string.IsNullOrEmpty(arguments.ConnectionProvider)
                    ? arguments.ConnectionProvider : template.ConnectionProvider;

                var connection = this.SavedConnectionService.CreateSavedConnection(
                    arguments.Name,
                    connectionString,
                    provider,
                    driverClass,
                    dialect);
                this.SavedConnectionService.SaveConnections();

                this.MessageService.WriteLine(
                    string.Format(
                        "Created a new connection \"{0}\" based on \"{1}\".", arguments.Name, arguments.TemplateConnection));
                this.WriteConnectionInfo(connection);
            }
            else
            {
                this.MessageService.WriteLine(
                string.Format("Template connection \"{0}\" could not be found.", arguments.TemplateConnection));
            }
        }

        /// <summary>
        /// Creates the new connection.
        /// </summary>
        /// <param name='arguments'>
        /// The arguments to create the connection with.
        /// </param>
        /// <returns>
        /// The new connection.
        /// </returns>
        private void CreateNewConnection(SavedConnectionArguments arguments)
        {
            var existingConnection =
                this.SavedConnectionService.SavedConnections.FirstOrDefault(c => c.Name == arguments.Name);

            string name;
            string connectionString;
            string connectionProvider;
            string driverClass;
            string dialect;

            if (existingConnection != null)
            {
                name = existingConnection.Name;
                connectionString = !string.IsNullOrEmpty(arguments.ConnectionString)
                    ? arguments.ConnectionString : existingConnection.ConnectionString;
                connectionProvider = !string.IsNullOrEmpty(arguments.ConnectionProvider)
                    ? arguments.ConnectionProvider : existingConnection.ConnectionProvider;
                driverClass = !string.IsNullOrEmpty(arguments.DriverClass)
                    ? arguments.DriverClass : existingConnection.DriverClass;
                dialect = !string.IsNullOrEmpty(arguments.Dialect)
                    ? arguments.Dialect : existingConnection.Dialect;
            }
            else
            {
                name = arguments.Name;
                connectionString = arguments.ConnectionString;
                connectionProvider = arguments.ConnectionProvider;
                driverClass = arguments.DriverClass;
                dialect = arguments.Dialect;
            }

            var connection = this.SavedConnectionService.CreateSavedConnection(
                name,
                connectionString,
                connectionProvider,
                driverClass,
                dialect);
            this.SavedConnectionService.SaveConnections();

            this.MessageService.WriteLine(
                string.Format("Created new saved connection \"{0}\":", connection.Name));
            this.WriteConnectionInfo(connection);
        }

        private void SetDefaultConnection (SavedConnectionArguments arguments)
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

        private void DeleteConnection (SavedConnectionArguments arguments)
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

        private void ListConnections (SavedConnectionArguments arguments)
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

