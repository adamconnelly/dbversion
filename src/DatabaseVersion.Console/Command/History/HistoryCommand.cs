namespace dbversion.Console.Command.History
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;
    using dbversion.Version;

    using Session = dbversion.Session;

    /// <summary>
    /// A command for outputting the history of versions installed in the database.
    /// </summary>
    [Export(typeof(IConsoleCommand))]
    public class HistoryCommand : IConsoleCommand
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string Name
        {
            get
            {
                return "history";
            }
        }

        /// <summary>
        /// Gets a description of the command.
        /// </summary>
        public string Description
        {
            get
            {
                return "Prints out a history of the installed versions.";
            }
        }

        /// <summary>
        /// Gets the usage of the command.
        /// </summary>
        public string Usage
        {
            get
            {
                return string.Format("dbversion {0} [options]", this.Name);
            }
        }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        public IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect."),
                    new CommandParameter("-o", "--order", "The order to sort the versions by [asc|desc]."),
                    new CommandParameter("-t", "--showTasks", "Indicates whether the tasks for each version should be output.")
                };
            }
        }

        /// <summary>
        /// Gets or sets the message service.
        /// </summary>
        [Import]
        public IMessageService MessageService { get; set; }

        /// <summary>
        /// Gets or sets the session factory provider.
        /// </summary>
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider { get; set; }

        /// <summary>
        /// Gets or sets the version provider.
        /// </summary>
        [Import]
        public IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the property service.
        /// </summary>
        [Import]
        public IPropertyService PropertyService { get; set; }

        /// <summary>
        /// Gets or sets the settings service.
        /// </summary>
        [Import]
        public ISettingsService SettingsService { get; set; }

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        public void Execute (string[] args)
        {
            var arguments = ParseArguments(args);

            this.PropertyService.SetDefaultProperties();
            this.MergePropertiesFromSettings();
            this.SetPropertiesFromCommandArguments(arguments);

            using (var sessionFactory = this.SessionFactoryProvider.CreateSessionFactory())
            {
                using (var session = sessionFactory.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        foreach (var version in this.GetSortedVersions(arguments, session))
                        {
                            this.WriteVersion(version, arguments);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the versions sorted in the correct order.
        /// </summary>
        /// <returns>
        /// The sorted versions.
        /// </returns>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        /// <param name='session'>
        /// The NHibernate session.
        /// </param>
        private IEnumerable<VersionBase> GetSortedVersions(HistoryArguments arguments, NHibernate.ISession session)
        {
            var versions = this.VersionProvider.GetAllVersions(session);
            if (arguments.SortOrder == HistoryOrder.asc || arguments.SortOrder == HistoryOrder.Ascending)
            {
                return versions.OrderBy(v => v, this.VersionProvider.GetComparer());
            }
            else
            {
                return versions.OrderByDescending(v => v, this.VersionProvider.GetComparer());
            }
        }

        /// <summary>
        /// Writes the specified version.
        /// </summary>
        /// <param name='version'>
        /// The version to write.
        /// </param>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        private void WriteVersion (VersionBase version, HistoryArguments arguments)
        {
            string versionString = string.Format(
                "{0} Installed - {1}, Updated - {2}",
                version.VersionText,
                version.CreatedOnLocal,
                version.UpdatedOnLocal);

            this.MessageService.WriteLine(versionString);

            if (arguments.ShowTasks)
            {
                this.WriteTasks(version, arguments.SortOrder);
            }
        }

        /// <summary>
        /// Writes the tasks.
        /// </summary>
        /// <param name='version'>
        /// The version containing the tasks.
        /// </param>
        /// <param name='sortOrder'>
        /// The order to write the tasks in.
        /// </param>
        private void WriteTasks(VersionBase version, HistoryOrder sortOrder)
        {
            IEnumerable<Task> sortedTasks;
            if (sortOrder == HistoryOrder.asc || sortOrder == HistoryOrder.Ascending)
            {
                sortedTasks = version.Tasks.OrderBy(t => t.ExecutionOrder);
            }
            else
            {
                sortedTasks = version.Tasks.OrderByDescending(t => t.ExecutionOrder);
            }

            foreach (var task in sortedTasks)
            {
                string taskString = string.Format(
                    "  {0} - {1}",
                    task.Name,
                    task.UpdatedOnLocal);

                this.MessageService.WriteLine(taskString);
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
        private static HistoryArguments ParseArguments(string[] args)
        {
            HistoryArguments arguments = new HistoryArguments();
            var parserSettings = new CommandLineParserSettings();
            parserSettings.CaseSensitive = true;
            parserSettings.HelpWriter = System.Console.Out;

            var parser = new CommandLineParser(parserSettings);
            parser.ParseArguments(args, arguments);

            return arguments;
        }

        /// <summary>
        /// Merges the properties from user settings.
        /// </summary>
        private void MergePropertiesFromSettings()
        {
            var propertyCollection = this.SettingsService.DeSerialize<PropertyCollection>("properties.xml");
            if (propertyCollection != null)
            {
                this.PropertyService.Merge(propertyCollection.Properties);
            }
        }

        /// <summary>
        /// Sets the properties from the command arguments.
        /// </summary>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        private void SetPropertiesFromCommandArguments(HistoryArguments arguments)
        {
            if (!string.IsNullOrEmpty(arguments.ConnectionString))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = Session.SessionFactoryProvider.ConnectionStringProperty,
                     Value = arguments.ConnectionString
                 });
            }

            if (!string.IsNullOrEmpty(arguments.ConnectionProvider))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = Session.SessionFactoryProvider.ProviderProperty,
                     Value = arguments.ConnectionProvider
                 });
            }
         
            if (!string.IsNullOrEmpty(arguments.DriverClass))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = Session.SessionFactoryProvider.DriverClassProperty,
                     Value = arguments.DriverClass
                 });
            }
         
            if (!string.IsNullOrEmpty(arguments.Dialect))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = Session.SessionFactoryProvider.DialectProperty,
                     Value = arguments.Dialect
                 });
            }
        }
    }
}

