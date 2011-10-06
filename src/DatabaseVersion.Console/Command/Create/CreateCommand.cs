namespace dbversion.Console.Command.Create
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Archives;
    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;
    using dbversion.Tasks;
    using dbversion.Version;

    [Export(typeof(IConsoleCommand))]
    public class CreateCommand : IConsoleCommand
    {
        public string Name
        {
            get
            {
                return "create";
            }
        }

        public string Description
        {
            get
            {
                return "Creates or upgrades a database using the specified archive.";
            }
        }

        public string Usage
        {
            get
            {
                return "dbversion " + this.Name + " [options]";
            }
        }

        public IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-a", "--archive", "The archive to create the database from."),
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-v", "--version", "The version to create or upgrade to."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect.")
                };
            }
        }

        [Import]
        public IDatabaseCreator Creator
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

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        public void Execute(string[] args)
        {
            Arguments arguments = ParseArguments(args);
            if (string.IsNullOrEmpty(arguments.Archive))
            {
                this.MessageService.WriteLine("Please specify an archive using the -a switch");
                return;
            }

            var archive = GetArchive(arguments.Archive);
            if (archive == null)
            {
                this.MessageService.WriteLine("The specified archive is not supported");
                return;
            }

            this.PropertyService.SetDefaultProperties();
            this.MergePropertiesFromSettings();
            this.PropertyService.Merge(archive.Properties);
            this.SetPropertiesFromCommandArguments(arguments);

            try
            {
                this.Creator.Create(archive, arguments.Version, new ConsoleTaskExecuter(MessageService));
            }
            catch (VersionNotFoundException v)
            {
                this.MessageService.WriteLine(v.Message);
            }
            catch (TaskExecutionException t)
            {
                this.MessageService.WriteLine(t.Message);
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
        private static Arguments ParseArguments(string[] args)
        {
            Arguments arguments = new Arguments();
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
        private void SetPropertiesFromCommandArguments(Arguments arguments)
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
    }
}
