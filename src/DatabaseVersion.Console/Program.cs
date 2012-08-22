namespace dbversion.Console
{
    using System;
    using System.Reflection;
    using System.IO;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition;

    using dbversion.Console.Command;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

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
            var container = CreateContainer();
            var commandManager = container.GetExportedValue<ICommandManager>();
            bool result = commandManager.Execute(args);
            Environment.ExitCode = result ? 0 : 1;
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
        private static CompositionContainer CreateContainer()
        {
            var catalogs = new List<ComposablePartCatalog>
            {
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new AssemblyCatalog(typeof(DatabaseCreator).Assembly),
            };

            var fileInfo = new FileInfo(typeof(Program).Assembly.Location);
            DirectoryInfo pluginPathInfo = new DirectoryInfo(Path.Combine(fileInfo.DirectoryName, "plugins"));
            if (pluginPathInfo.Exists)
            {
                catalogs.Add(new DirectoryCatalog(pluginPathInfo.FullName));
            }

            var aggregateCatalog = new AggregateCatalog(catalogs);

            return new CompositionContainer(aggregateCatalog);
        }
    }
}
