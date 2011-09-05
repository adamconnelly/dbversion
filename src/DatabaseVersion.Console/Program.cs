namespace dbversion.Console
{
    using System;
    using System.Reflection;
    using System.IO;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition;

    using dbversion.Console.Command;

    /// <summary>
    /// Contains the entry point of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            var container = CreateContainer("plugins");
            var commandManager = container.GetExportedValue<ICommandManager>();
            commandManager.Execute(args);
        }

        /// <summary>
        /// Creates the MEF container.
        /// </summary>
        /// <param name='pluginPath'>
        /// The path to load plugins from.
        /// </param>
        /// <returns>
        /// The MEF container.
        /// </returns>
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
