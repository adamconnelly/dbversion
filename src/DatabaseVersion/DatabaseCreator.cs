using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseVersion.Archives;
using System.Reflection;
using System.IO;
using System.Data;
using DatabaseVersion.Manifests;
using DatabaseVersion.Connections;
using System.ComponentModel.Composition;

namespace DatabaseVersion
{
    public class DatabaseCreator
    {
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
        /// Creates a database at the specified version or upgrades the existing database to the specified version.
        /// </summary>
        /// <param name="version">The version of database to create.</param>
        /// <exception cref="VersionNotFoundException">
        /// Thrown if the version to create could not be found.
        /// </exception>
        /// <exception cref="TaskExecutionException">
        /// Thrown if an error occurs while executing one of the tasks in the archive.
        /// </exception>
        public void Create(string version, string connectionString, string connectionType)
        {
            IDbConnection connection = this.ConnectionFactory.Create(connectionString, connectionType);
            connection.Open();

            if (!this.VersionProvider.VersionTableExists(connection))
            {
                this.VersionProvider.CreateVersionTable(connection);
            }

            object currentVersion = this.VersionProvider.GetCurrentVersion(connection);

            object targetVersion;
            if (string.IsNullOrEmpty(version))
            {
                targetVersion = this.Archive.Versions
                    .OrderByDescending(v => v.Version, this.VersionProvider.GetComparer())
                    .First()
                    .Version;
            }
            else
            {
                targetVersion = this.VersionProvider.CreateVersion(version);
            }

            if (!this.Archive.ContainsVersion(targetVersion))
            {
                throw new Version.VersionNotFoundException(targetVersion);
            }

            IEnumerable<IDatabaseVersion> versionsToExecute = this.Archive.Versions
                .OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                .Where(v => currentVersion == null || this.VersionProvider.GetComparer().Compare(currentVersion, v.Version) < 0)
                .TakeWhile(v => this.VersionProvider.GetComparer().Compare(targetVersion, v.Version) >= 0);

            foreach (IDatabaseVersion v in versionsToExecute)
            {
                foreach (var task in v.Tasks.OrderBy(t => t.ExecutionOrder))
                {
                    task.Execute(connection);
                }

                this.VersionProvider.InsertVersion(v.Version, connection);
            }

            connection.Close();
        }

        public void LoadArchive(string archivePath)
        {
            IDatabaseArchiveFactory archiveFactory = this.GetArchiveFactory(archivePath);
            if (archiveFactory == null)
            {
                throw new InvalidOperationException("Unknown archive type");
            }

            this.Archive = archiveFactory.Create(archivePath);
        }

        private IDatabaseArchiveFactory GetArchiveFactory(string archivePath)
        {
            // TODO: Throw UnknownArchiveTypeException if no handlers found
            return this.ArchiveFactories.First(f => f.CanCreate(archivePath));
        }
    }
}
