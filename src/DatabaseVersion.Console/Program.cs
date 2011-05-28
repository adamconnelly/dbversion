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

            try
            {
                args = parser.Parse();
            }
            catch (Exception)
            {
                UsageBuilder usageBuilder = new UsageBuilder();
                usageBuilder.BeginSection("Name");
                usageBuilder.AddParagraph(Assembly.GetExecutingAssembly().GetName().Name + " - Database Creator / Upgrader");
                usageBuilder.EndSection();

                usageBuilder.BeginSection("Arguments");
                usageBuilder.AddOptions(parser);
                usageBuilder.EndSection();

                System.Console.WriteLine(usageBuilder.ToString());
                Environment.Exit(0);
            }

            DatabaseCreator creator = new DatabaseCreator("plugins", arguments.Archive);

            if (arguments.CreationMode == CreationMode.Create)
            {
                creator.Create(arguments.Version);
            }
            else
            {
                creator.Upgrade(arguments.Version);
            }
        }
    }
}
