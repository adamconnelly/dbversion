using CommandLine.OptParse;
using System;
using System.Reflection;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using DatabaseVersion.Version;
using System.Linq;
using DatabaseVersion.Tasks;

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
                creator.LoadArchive(arguments.Archive);
                creator.Create(arguments.Version, arguments.ConnectionString, arguments.ConnectionType);
            }
            catch (VersionNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (TaskExecutionException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private static Arguments ParseArguments(ref string[] args)
        {
            Arguments arguments = new Arguments();
            Parser parser = ParserFactory.BuildParser(arguments);
            parser.OptStyle = OptStyle.Unix;

            if (args.Length <= 0)
            {
                PrintUsage(parser);
            }

            try
            {
                args = parser.Parse();
            }
            catch (Exception)
            {
                PrintUsage(parser);
            }
            return arguments;
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

        private static void PrintUsage(Parser parser)
        {
            UsageBuilder usageBuilder = new UsageBuilder();
            usageBuilder.BeginSection("Name");
            usageBuilder.AddParagraph(Assembly.GetExecutingAssembly().GetName().Name + " - Database Creator / Upgrader");
            usageBuilder.EndSection();

            usageBuilder.BeginSection("Arguments");
            usageBuilder.AddOptions(parser);
            usageBuilder.EndSection();

            usageBuilder.ToText(System.Console.Out, OptStyle.Unix, true);
            Environment.Exit(0);
        }
    }
}
