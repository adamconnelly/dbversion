using dbversion.Archives;

namespace dbversion.Console
{
    using System;
    using System.Reflection;
    using System.IO;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Console.Command;
    using dbversion.Property;
    using dbversion.Settings;
    using dbversion.Tasks;
    using dbversion.Version;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = CreateContainer("plugins");
            var commandManager = container.GetExportedValue<ICommandManager>();
            commandManager.Execute(args);
//            Arguments arguments = ParseArguments(ref args);
//            var container = CreateContainer("plugins");
//
//            DatabaseCreator creator = new DatabaseCreator();
//            container.ComposeParts(creator);
//
//            try
//            {
//                var archive = GetArchive(arguments, container);
//                var propertyService = container.GetExportedValue<IPropertyService>();
//                propertyService.SetDefaultProperties();
//                MergeSavedProperties(container, propertyService);
//                propertyService.Merge(archive.Properties);
//                OverwritePropertiesFromArguments(propertyService, arguments);
//
//                creator.Create(archive, arguments.Version, new ConsoleTaskExecuter());
//            } catch (VersionNotFoundException e)
//            {
//                System.Console.WriteLine(e.Message);
//            } catch (TaskExecutionException e)
//            {
//                System.Console.WriteLine(e.Message);
//            }
        }

        private static IDatabaseArchive GetArchive(Arguments arguments, CompositionContainer container)
        {
            IDatabaseArchiveFactory archiveFactory = GetArchiveFactory(arguments.Archive, container);
            if (archiveFactory == null)
            {
                throw new InvalidOperationException("Unknown archive type");
            }

            return archiveFactory.Create(arguments.Archive);
        }

        private static IDatabaseArchiveFactory GetArchiveFactory(string archivePath, CompositionContainer container)
        {
            var archiveFactories = container.GetExportedValues<IDatabaseArchiveFactory>();
            // TODO: Throw UnknownArchiveTypeException if no handlers found
            return archiveFactories.First(f => f.CanCreate(archivePath));
        }

        private static Arguments ParseArguments(ref string[] args)
        {
            Arguments arguments = new Arguments();
            var parserSettings = new CommandLineParserSettings();
            parserSettings.CaseSensitive = true;
            parserSettings.HelpWriter = System.Console.Out;

            var parser = new CommandLineParser(parserSettings);

            if (args.Length == 0)
            {
                System.Console.WriteLine(arguments.GetHelp());
                Environment.Exit(0);
            }

            if (parser.ParseArguments(args, arguments))
            {
                return arguments;
            }

            Environment.Exit(0);
            return null;
        }

        private static CompositionContainer CreateContainer(string pluginPath)
        {
            DirectoryInfo pluginPathInfo = new DirectoryInfo(pluginPath);
            if (!pluginPathInfo.Exists)
            {
                pluginPathInfo.Create();
            }

            var aggregateCatalog = new AggregateCatalog(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new AssemblyCatalog(typeof(DatabaseCreator).Assembly),
                new DirectoryCatalog(pluginPath));

            return new CompositionContainer(aggregateCatalog);
        }

        private static void OverwritePropertiesFromArguments(IPropertyService propertyService, Arguments arguments)
        {
            propertyService.Add(new Property { Key = "hibernate.connection.connection_string", Value = arguments.ConnectionString });
        }

        private static void MergeSavedProperties(CompositionContainer container, IPropertyService propertyService)
        {
            var settingsService = container.GetExportedValue<ISettingsService>();
            var savedProperties = settingsService.DeSerialize<PropertyCollection>("properties.xml");
         
            if (savedProperties != null)
            {
                propertyService.Merge(savedProperties.Properties);
            }
        }
    }
}
