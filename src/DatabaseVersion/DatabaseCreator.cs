using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbversion.Archives;
using System.Reflection;
using System.IO;
using System.Data;
using dbversion.Manifests;
using dbversion.Session;
using System.ComponentModel.Composition;
using dbversion.Tasks;
using dbversion.Tasks.Version;
using dbversion.Version.ClassicVersion;
using dbversion.Version;

namespace dbversion
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
        public ISessionFactoryProvider SessionFactoryProvider
        {
            get;
            set;
        }

        public IDatabaseArchive Archive
        {
            get;
            private set;
        }

        public void Create(string version, string connectionString)
        {
            this.Create(version, connectionString, new SimpleTaskExecuter());
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
        public void Create(string version, string connectionString, ITaskExecuter executer)
        {
            using (var sessionFactory = this.SessionFactoryProvider.CreateSessionFactory())
            {
                using (var session = sessionFactory.OpenSession())
                {
                    // Set the session to always flush to make sure we execute everything
                    // in order
                    session.FlushMode = NHibernate.FlushMode.Always;

                    using (var transaction = session.BeginTransaction())
                    {
                        if (!this.VersionProvider.VersionTableExists(session))
                        {
                            this.VersionProvider.CreateVersionTable(session);
                        }

                        VersionBase currentVersion = this.VersionProvider.GetCurrentVersion(session);

                        object targetVersion;
                        if (string.IsNullOrEmpty(version))
                        {
                            targetVersion = this.Archive.Versions
                                .OrderByDescending(v => v.Version, this.VersionProvider.GetComparer())
                                .First()
                                .Version;
                        } else
                        {
                            targetVersion = this.VersionProvider.CreateVersion(version);
                        }

                        if (!this.Archive.ContainsVersion(targetVersion))
                        {
                            throw new Version.VersionNotFoundException(targetVersion);
                        }

                        this.AddTasksToExecuter(executer, currentVersion, targetVersion);

                        executer.ExecuteTasks(session);

                        transaction.Commit();
                    }
                }
            }
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

        private void AddTasksToExecuter(ITaskExecuter executer, VersionBase currentVersion, object targetVersion)
        {
            IEnumerable<IDatabaseVersion > versionsToExecute = this.Archive.Versions
                .OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                .Where(
                    v =>
                    currentVersion == null ||
                    this.VersionProvider.GetComparer().Compare(currentVersion, v.Version) <= 0)
                .TakeWhile(v => this.VersionProvider.GetComparer().Compare(targetVersion, v.Version) >= 0);

            foreach (IDatabaseVersion v in versionsToExecute)
            {
                bool updating = ((currentVersion != null) && currentVersion.Equals(v.Version));
                foreach (var task in v.Tasks.OrderBy(t => t.ExecutionOrder))
                {
                    //Check if we're updating an existing version or inserting a new one
                    if (updating)
                    {
                        if (!this.VersionProvider.HasExecutedScript(currentVersion, v.Version, task))
                        {
                            executer.AddTask(task);
                            currentVersion.AddTask(task);
                        }
                    } else
                    {
                        executer.AddTask(task);
                        v.Version.AddTask(task);
                    }
                }

                if (executer.HasTasks)
                {
                    if (updating)
                    {
                        executer.AddTask(new InsertVersionTask(this.VersionProvider, currentVersion));
                    } else
                    {
                        executer.AddTask(new InsertVersionTask(this.VersionProvider, v.Version));
                    }
                }
            }
        }

        private IDatabaseArchiveFactory GetArchiveFactory(string archivePath)
        {
            // TODO: Throw UnknownArchiveTypeException if no handlers found
            return this.ArchiveFactories.First(f => f.CanCreate(archivePath));
        }
    }
}
