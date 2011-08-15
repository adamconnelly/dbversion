using System;
using System.Reflection;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using DatabaseVersion.Version;
using System.Linq;
using DatabaseVersion.Tasks;
using CommandLine;
using DatabaseVersion.Property;

namespace DatabaseVersion.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Arguments arguments = ParseArguments(ref args);
            var container = CreateContainer(arguments.PluginDirectory);

            DatabaseCreator creator = new DatabaseCreator();
            container.ComposeParts(creator);

            if (arguments.ListConnectionTypes)
            {
                foreach (var connectionCreator in creator.ConnectionFactory.Creators)
                {
                    System.Console.WriteLine(connectionCreator.ConnectionType);
                }

                Environment.Exit(0);
            }

            try
            {
                var propertyService = container.GetExportedValue<IPropertyService>();
                propertyService["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
                propertyService["connection.driver_class"] = "NHibernate.Driver.MySqlDataDriver";
                propertyService["connection.connection_string"] = "Server=localhost;Database=library;Uid=adam;";
                propertyService["dialect"] = "NHibernate.Dialect.MySQLDialect";

                creator.LoadArchive(arguments.Archive);
                creator.Create(arguments.Version, arguments.ConnectionString, arguments.ConnectionType, new ConsoleTaskExecuter());
            } catch (VersionNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
            } catch (TaskExecutionException e)
            {
                System.Console.WriteLine(e.Message);
            }
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
    }
}
