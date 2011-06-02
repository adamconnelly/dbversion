using CommandLine.OptParse;
using System;
using System.Reflection;
namespace DatabaseVersion.Console
{
    public class Program
    {
        public static void Main(string[] args)
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

            DatabaseCreator creator = new DatabaseCreator("plugins", arguments.Archive);

            if (arguments.CreationMode == CreationMode.Create)
            {
                creator.Create(arguments.Version, arguments.ConnectionString, arguments.ConnectionType);
            }
            else
            {
                creator.Upgrade(arguments.Version);
            }
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
