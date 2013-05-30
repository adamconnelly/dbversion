namespace dbversion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.Composition;
    using NHibernate;
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

        private void LogUpdateComplete(bool commit)
        {
            MessageService.WriteLine(String.Format("{0} Database Update. Time Taken: {1}", 
                commit ? "Finished" : "Rolled back", 
                DateTime.Now.Subtract(_updateStartTime)));
        }

        #endregion

        /// <summary>
        /// Creates a database at the specified version or upgrades the existing database to the specified version.
        /// </summary>
        /// <param name="archive">The archive containing the tasks to run.</param>
        /// <param name="version">The version of database to create.</param>
        /// <param name="executer">The object used to execute the tasks.</param>
        /// <param name="commit">true if any changes should be committed, false if they should be rolled back.</param>
        /// <param name="executeMissingTasks">true if any missing tasks detected should be executed, otherwise false.</param>
        /// <returns>Returns the result of Executing the Command</returns>
        /// <exception cref="VersionNotFoundException">
        /// Thrown if the version to create could not be found.
        /// </exception>
        /// <exception cref="TaskExecutionException">
        /// Thrown if an error occurs while executing one of the tasks in the archive.
        /// </exception>
        public bool Create(IDatabaseArchive archive, string version, ITaskExecuter executer, bool commit, bool executeMissingTasks)
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

                        VersionBase targetVersion;
                        bool targetVersionSpecified = !string.IsNullOrEmpty(version);
                        if (!targetVersionSpecified)
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

                        this.AddTasksToExecuter(archive, executer, currentVersion, targetVersion, executeMissingTasks,
                                                session, targetVersionSpecified);

                        executer.ExecuteTasks(session);

                        if (commit)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                        LogUpdateComplete(commit);           
                    }
                }
            }
            return true;
        }

        private void AddTasksToExecuter(IDatabaseArchive archive, ITaskExecuter executer, VersionBase currentVersion, VersionBase targetVersion, bool executeMissingTasks, ISession session, bool targetVersionSpecified)
        {
            if (executeMissingTasks)
            {
                this.AddAllTasks(archive, executer, targetVersion, session, targetVersionSpecified);
            }
            else
            {
                this.AddNewTasks(archive, executer, currentVersion, targetVersion, session);
            }
        }

        private void AddAllTasks(IDatabaseArchive archive, ITaskExecuter executer, VersionBase targetVersion, ISession session, bool targetVersionSpecified)
        {
            // If we're executing missing tasks, we need to go through each version and check whether any tasks are missing
            // In this case, if the target version is specified, we only look in that version
            var versions =
                archive.Versions.OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                       .Where(
                           v =>
                           !targetVersionSpecified || this.VersionProvider.GetComparer().Compare(targetVersion, v.Version) == 0);

            this.AddTasksForVersions(executer, versions, session);
        }

        private void AddNewTasks(IDatabaseArchive archive, ITaskExecuter executer, VersionBase currentVersion,
                                    VersionBase targetVersion, ISession session)
        {
            IEnumerable<IDatabaseVersion> versionsToExecute = archive.Versions
                                                                     .OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                                                                     .Where(
                                                                         v =>
                                                                         currentVersion == null ||
                                                                         this.VersionProvider.GetComparer()
                                                                             .Compare(currentVersion, v.Version) <= 0)
                                                                     .TakeWhile(
                                                                         v =>
                                                                         this.VersionProvider.GetComparer()
                                                                             .Compare(targetVersion, v.Version) >= 0);

            this.AddTasksForVersions(executer, versionsToExecute, session);
        }

        private void AddTasksForVersions(ITaskExecuter executer, IEnumerable<IDatabaseVersion> versionsToExecute, ISession session)
        {
            foreach (IDatabaseVersion v in versionsToExecute)
            {
                bool hasTasksForVersion = false;

                var installedVersion = this.VersionProvider.GetVersion(session, v.Version);
                var updateVersion = installedVersion ?? v.Version;

                foreach (var task in v.Tasks.OrderBy(t => t.ExecutionOrder))
                {
                    if (!this.VersionProvider.HasExecutedScript(installedVersion, v.Version, task))
                    {
                        executer.AddTask(task);
                        updateVersion.AddTask(task);
                        hasTasksForVersion = true;
                    }
                }

                if (hasTasksForVersion)
                {
                    executer.AddTask(new InsertVersionTask(this.VersionProvider, updateVersion, this.MessageService));
                }
            }
        }
    }
}
