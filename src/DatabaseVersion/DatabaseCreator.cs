namespace dbversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.IO;
    using System.ComponentModel.Composition;

    using dbversion.Archives;
    using dbversion.Manifests;
    using dbversion.Session;
    using dbversion.Tasks;
    using dbversion.Tasks.Version;
    using dbversion.Version;

    [Export(typeof(IDatabaseCreator))]
    public class DatabaseCreator : IDatabaseCreator
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

        [Import]
        public IMessageService MessageService
        {
            get;
            set;
        }

        #region Logging

        private DateTime _updateStartTime;
        private void LogUpdateStart()
        {
            _updateStartTime = DateTime.Now;
            MessageService.WriteLine("Starting Database Update");
        }

        private void LogUpdateComplete()
        {
            MessageService.WriteLine(String.Format("Finished Database Update. Time Taken: {0}", DateTime.Now.Subtract(_updateStartTime)));
        }

        #endregion

        /// <summary>
        /// Creates a database at the specified version or upgrades the existing database to the specified version.
        /// </summary>
        /// <param name="version">The version of database to create.</param>
        /// <returns>Returns the result of Executing the Command</returns>
        /// <exception cref="VersionNotFoundException">
        /// Thrown if the version to create could not be found.
        /// </exception>
        /// <exception cref="TaskExecutionException">
        /// Thrown if an error occurs while executing one of the tasks in the archive.
        /// </exception>
        public bool Create(IDatabaseArchive archive, string version, ITaskExecuter executer)
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
                        LogUpdateStart();

                        if (!this.VersionProvider.VersionTableExists(session))
                        {
                            this.VersionProvider.CreateVersionTable(session);
                        }

                        VersionBase currentVersion = this.VersionProvider.GetCurrentVersion(session);

                        //Log the current version
                        MessageService.WriteLine(currentVersion == null
                                                     ? "Current Database Version Unknown"
                                                     : string.Format("Current Database Version: {0}", currentVersion));

                        object targetVersion;
                        if (string.IsNullOrEmpty(version))
                        {
                            targetVersion = archive.Versions
                                .OrderByDescending(v => v.Version, this.VersionProvider.GetComparer())
                                .First()
                                .Version;
                        }
                        else
                        {
                            targetVersion = this.VersionProvider.CreateVersion(version);
                        }

                        MessageService.WriteLine(string.Format("Target Version: {0}", targetVersion));

                        if (!archive.ContainsVersion(targetVersion))
                        {
                            MessageService.WriteLine(string.Format("Target Version Not Found in Archive"));
                            throw new VersionNotFoundException(targetVersion);
                        }

                        this.AddTasksToExecuter(archive, executer, currentVersion, targetVersion);

                        executer.ExecuteTasks(session);

                        transaction.Commit();

                        LogUpdateComplete();
                    }
                }
            }
            return true;
        }

        private void AddTasksToExecuter(IDatabaseArchive archive, ITaskExecuter executer, VersionBase currentVersion, object targetVersion)
        {
            IEnumerable<IDatabaseVersion > versionsToExecute = archive.Versions
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
                    }
                    else
                    {
                        executer.AddTask(task);
                        v.Version.AddTask(task);
                    }
                }

                if (executer.HasTasks)
                {
                    if (updating)
                    {
                        executer.AddTask(new InsertVersionTask(this.VersionProvider, currentVersion, MessageService));
                    }
                    else
                    {
                        executer.AddTask(new InsertVersionTask(this.VersionProvider, v.Version, MessageService));
                    }
                }
            }
        }
    }
}
