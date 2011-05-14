using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using DatabaseVersion.Archives;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;

namespace DatabaseVersion
{
    public class DatabaseCreator
    {
        private readonly string connectionString;

        public DatabaseCreator(string pluginPath)
        {
            DirectoryInfo pluginPathInfo = new DirectoryInfo(pluginPath);
            if (!pluginPathInfo.Exists)
            {
                pluginPathInfo.Create();
            }

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var directoryCatalog = new DirectoryCatalog(pluginPath);
            var aggregateCatalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

            var container = new CompositionContainer(aggregateCatalog);
            container.ComposeParts(this);
        }

        [ImportMany]
        public IEnumerable<IDatabaseArchiveFactory> ArchiveFactories
        {
            get;
            set;
        }

        [Import]
        public IVersionProvider VersionProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a database at the specified version.
        /// </summary>
        /// <param name="version">The version of database to create.</param>
        public void Create(string version)
        {
        }

        public void Upgrade()
        {
        }

        public void Upgrade(string version)
        {
        }
    }
}
