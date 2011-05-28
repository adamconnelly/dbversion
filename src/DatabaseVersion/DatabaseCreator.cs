using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using DatabaseVersion.Archives;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;
using System.Data;
using DatabaseVersion.Manifests;
using DatabaseVersion.Connections;

namespace DatabaseVersion
{
    public class DatabaseCreator
    {
        public DatabaseCreator(string pluginPath, string archivePath)
        {
            this.CreateContainer(pluginPath);
            this.CreateArchive(archivePath);
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

        [Import]
        public IManifestReader Reader
        {
            get;
            set;
        }

        [Import]
        public IDbConnectionFactory ConnectionFactory
        {
            get;
            set;
        }

        public IDatabaseArchive Archive
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a database at the specified version.
        /// </summary>
        /// <param name="version">The version of database to create.</param>
        public void Create(string version, string connectionString)
        {
            IDbConnection connection = this.ConnectionFactory.Create(connectionString);
            //object currentVersion = this.VersionProvider.GetCurrentVersion(connection);

            // Throw exception if currentVersion exists because we are creating a database from scratch

            object targetVersion = this.VersionProvider.CreateVersion(version);

            // Throw VersionNotFoundException if targetVersion can't be found in the list of versions

            IEnumerable<IDatabaseVersion> versionsToExecute = this.Archive.Versions
                .OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                .TakeWhile(v => this.VersionProvider.GetComparer().Compare(targetVersion, v.Version) >= 0);

            foreach (IDatabaseVersion v in versionsToExecute)
            {
                foreach (var task in v.Tasks.OrderBy(t => t.ExecutionOrder))
                {
                    //task.Execute(connection);
                }
            }
        }

        public void Upgrade()
        {
        }

        public void Upgrade(string version)
        {
        }

        private IDatabaseArchiveFactory GetArchiveFactory(string archivePath)
        {
            // TODO: Throw UnknownArchiveTypeException if no handlers found
            return this.ArchiveFactories.First(f => f.CanCreate(archivePath));
        }

        private void CreateContainer(string pluginPath)
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

        private void CreateArchive(string archivePath)
        {
            IDatabaseArchiveFactory archiveFactory = this.GetArchiveFactory(archivePath);
            if (archiveFactory == null)
            {
                throw new InvalidOperationException("Unknown archive type");
            }

            this.Archive = archiveFactory.Create(archivePath);
            IDatabaseVersion version = this.Archive.Versions.FirstOrDefault();
        }
    }
}
