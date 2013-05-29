namespace dbversion.Console.Command
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Archives;
    using dbversion.Connections;
    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;

    /// <summary>
    /// A base class for commands that need to connect to the database.
    /// </summary>
    public abstract class ConnectionCommandBase<T> : IConsoleCommand where T : ConnectionArguments, new()
    {
        [Import]
        public IMessageService MessageService
        {
            get;
            set;
        }

        [Import]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        [Import]
        public ISettingsService SettingsService
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

        [ImportMany]
        public IEnumerable<IDatabaseArchiveFactory> ArchiveFactories
        {
            get;
            set;
        }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual string Usage
        {
            get { return "dbversion " + this.Name + " [options]"; }
        }

        public abstract IEnumerable<CommandParameter> Parameters { get; }

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        /// <returns>Returns the result of Executing the Command</returns>
        public bool Execute(string[] args)
        {
            T arguments = ParseArguments(args);

            var archive = GetArchive(arguments.Archive);

            this.SavedConnectionService.LoadConnections();

            this.PropertyService.SetDefaultProperties();
            this.MergePropertiesFromSettings();

            if (archive != null)
            {
                this.PropertyService.Merge(archive.Properties);
            }

            this.MergeDefaultConnectionProperties();
            this.SetPropertiesFromCommandArguments(arguments);

            return this.Execute(args, arguments, archive);
        }

        protected abstract bool Execute(string[] args, T arguments, IDatabaseArchive archive);

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name='args'>
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The parsed arguments.
        /// </returns>
        private static T ParseArguments(string[] args)
        {
            T arguments = new T();
            var parserSettings = new CommandLineParserSettings();
            parserSettings.CaseSensitive = true;
            parserSettings.HelpWriter = System.Console.Out;

            var parser = new CommandLineParser(parserSettings);
            parser.ParseArguments(args, arguments);

            return arguments;
        }

        /// <summary>
        /// Gets the archive.
        /// </summary>
        /// <param name='archivePath'>
        /// Archive path.
        /// </param>
        /// <returns>
        /// The archive or null if none could be created.
        /// </returns>
        private IDatabaseArchive GetArchive(string archivePath)
        {
            IDatabaseArchiveFactory archiveFactory = GetArchiveFactory(archivePath);
            if (archiveFactory == null)
            {
                return null;
            }

            return archiveFactory.Create(archivePath);
        }

        /// <summary>
        /// Gets the archive factory.
        /// </summary>
        /// <param name='archivePath'>
        /// Archive path.
        /// </param>
        /// <returns>
        /// The archive factory or null if none can be found.
        /// </returns>
        private IDatabaseArchiveFactory GetArchiveFactory(string archivePath)
        {
            return this.ArchiveFactories.FirstOrDefault(f => f.CanCreate(archivePath));
        }

        /// <summary>
        /// Sets the properties from the command arguments.
        /// </summary>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        private void SetPropertiesFromCommandArguments(T arguments)
        {
            if (!string.IsNullOrEmpty(arguments.ConnectionString))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = SessionFactoryProvider.ConnectionStringProperty,
                     Value = arguments.ConnectionString
                 });
            }
         
            if (!string.IsNullOrEmpty(arguments.ConnectionProvider))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = SessionFactoryProvider.ProviderProperty,
                     Value = arguments.ConnectionProvider
                 });
            }
         
            if (!string.IsNullOrEmpty(arguments.DriverClass))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = SessionFactoryProvider.DriverClassProperty,
                     Value = arguments.DriverClass
                 });
            }
         
            if (!string.IsNullOrEmpty(arguments.Dialect))
            {
                this.PropertyService.Add(
                 new Property
                 {
                     Key = SessionFactoryProvider.DialectProperty,
                     Value = arguments.Dialect
                 });
            }

            if (!string.IsNullOrEmpty(arguments.SavedConnection))
            {
                var savedConnection =
                    this.SavedConnectionService.SavedConnections.FirstOrDefault(c => c.Name == arguments.SavedConnection);
                if (savedConnection != null)
                {
                    this.PropertyService.Merge(savedConnection.GetConnectionProperties());
                }
            }
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
        /// Merges the default connection properties.
        /// </summary>
        private void MergeDefaultConnectionProperties()
        {
            var defaultConnection = this.SavedConnectionService.DefaultConnection;
            if (defaultConnection != null)
            {
                this.PropertyService.Merge(defaultConnection.GetConnectionProperties());
            }
        }
    }
}

