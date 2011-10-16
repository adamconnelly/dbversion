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
                    new CommandParameter("-l", "--dialect", "The hibernate dialect."),
                    new CommandParameter("-o", "--order", "The order to sort the versions by [asc|desc].")
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
                        foreach (var version in this.GetSortedVersions(arguments, session))
                        {
                            this.WriteVersion(version);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        private IEnumerable<VersionBase> GetSortedVersions (HistoryArguments arguments, NHibernate.ISession session)
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

        private void WriteVersion (VersionBase currentVersion)
        {
            string versionString = string.Format(
                "{0} Installed - {1}, Updated - {2}",
                currentVersion.VersionText,
                currentVersion.CreatedOnLocal,
                currentVersion.UpdatedOnLocal);

            this.MessageService.WriteLine(versionString);
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

