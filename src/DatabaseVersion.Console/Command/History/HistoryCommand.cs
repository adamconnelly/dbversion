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

    [Export(typeof(IConsoleCommand))]
    public class HistoryCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "history";
            }
        }

        public string Description
        {
            get
            {
                return "Prints out a history of the installed versions.";
            }
        }

        public string Usage
        {
            get
            {
                return string.Format("dbversion {0} [options]", this.Name);
            }
        }

        public IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect.")
                };
            }
        }

        [Import]
        public IMessageService MessageService { get; set; }

        [Import]
        public ISessionFactoryProvider SessionFactoryProvider { get; set; }

        [Import]
        public IVersionProvider VersionProvider { get; set; }

        [Import]
        public IPropertyService PropertyService { get; set; }

        [Import]
        public ISettingsService SettingsService { get; set; }

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
                        var currentVersion = this.VersionProvider.GetCurrentVersion(session);

                        string versionString = string.Format(
                            "{0} Installed - {1}, Updated - {2}",
                            currentVersion.VersionText,
                            currentVersion.CreatedOnLocal,
                            currentVersion.UpdatedOnLocal);

                        this.MessageService.WriteLine(versionString);

                        transaction.Commit();
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

