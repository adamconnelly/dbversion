namespace dbversion.Console.Command.Check
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Archives;
    using Create;
    using NHibernate;
    using Session;
    using dbversion.Version;

    /// <summary>
    /// Checks whether all the versions and tasks have been installed in a database.
    /// </summary>
    [Export(typeof(IConsoleCommand))]
    public class CheckCommand : ConnectionCommandBase<CreateArguments>
    {
        /// <summary>
        /// Gets the name of the command. This is the name that will be used
        /// to execute the command at the command line.
        /// </summary>
        /// <value>
        /// The name of the command to execute.
        /// </value>
        public override string Name
        {
            get { return "check"; }
        }

        /// <summary>
        /// Gets a short description of what the command does.
        /// </summary>
        /// <value>
        /// The description of the command.
        /// </value>
        public override string Description
        {
            get { return "Checks whether all of the versions and tasks have been installed in the database."; }
        }

        /// <summary>
        /// Gets the parameters that can be used with the command.
        /// </summary>
        /// <value>
        /// The parameters or an empty enumerable.
        /// </value>
        public override IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-a", "--archive", "The archive to create the database from."),
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-v", "--version", "The version to create or upgrade to."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect."),
                    new CommandParameter("-s", "--saved-connection", "The name of the saved connection to use.")
                };
            }
        }

        [Import]
        public IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the session factory provider.
        /// </summary>
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider { get; set; }

        protected override bool Execute(string[] args, CreateArguments arguments, IDatabaseArchive archive)
        {
            if (archive == null)
            {
                this.MessageService.WriteLine("Please specify an archive using the -a switch.");
                return false;
            }

            return this.CheckDatabase(arguments, archive);
        }

        /// <summary>
        /// Checks whether the database has all the versions and tasks installed.
        /// </summary>
        /// <param name="arguments">The command line arguments.</param>
        /// <param name="archive">The database archive.</param>
        /// <returns>true if no errors occurred, otherwise false.</returns>
        private bool CheckDatabase(CreateArguments arguments, IDatabaseArchive archive)
        {
            bool errorOccurred = false;
            bool isDatabaseUpToDate = true;

            using (var sessionFactory = this.SessionFactoryProvider.CreateSessionFactory())
            using (var session = sessionFactory.OpenSession())
            using (session.BeginTransaction())
            {
                if (!string.IsNullOrEmpty(arguments.Version))
                {
                    isDatabaseUpToDate = CheckSpecificVersion(arguments, archive, session, out errorOccurred);
                }
                else
                {
                    foreach (var version in archive.Versions.OrderBy(v => v.Version, this.VersionProvider.GetComparer())
                        )
                    {
                        if (!this.CheckVersion(session, version))
                        {
                            isDatabaseUpToDate = false;
                        }
                    }
                }
            }

            if (isDatabaseUpToDate)
            {
                this.MessageService.WriteLine("The database is up to date.");
            }

            return !errorOccurred;
        }

        /// <summary>
        /// Checks whether a specific version has been installed in the database.
        /// </summary>
        /// <param name="arguments">The command line arguments</param>
        /// <param name="archive">The database archive.</param>
        /// <param name="session">The database session.</param>
        /// <param name="errorOccurred">true if an error occurs, otherwise false.</param>
        /// <returns>true if the version and all its tasks exist, otherwise false.</returns>
        private bool CheckSpecificVersion(CreateArguments arguments, IDatabaseArchive archive, ISession session, out bool errorOccurred)
        {
            errorOccurred = false;

            var targetVersion = this.VersionProvider.CreateVersion(arguments.Version);
            var archiveVersion = archive.Versions
                                        .SingleOrDefault(
                                            v =>
                                            this.VersionProvider.GetComparer().Compare(v.Version, targetVersion) ==
                                            0);

            if (archiveVersion != null)
            {
                return this.CheckVersion(session, archiveVersion);
            }

            this.MessageService.WriteLine(string.Format("Version {0} does not exist in the archive.",
                                                        arguments.Version));
            errorOccurred = true;

            return false;
        }

        /// <summary>
        /// Checks whether the specified version has been fully installed.
        /// </summary>
        /// <param name="session">The hibernate session.</param>
        /// <param name="version">The version to check.</param>
        /// <returns>true if the version exists and all its tasks have been installed, otherwise false.</returns>
        private bool CheckVersion(ISession session, IDatabaseVersion version)
        {
            var installedVersion = this.VersionProvider.GetVersion(session, version.Version);

            if (installedVersion == null)
            {
                this.MessageService.WriteLine(string.Format("Version {0} has not been installed.",
                                                            version.Version.VersionText));

                return false;
            }

            return CheckTasks(version, installedVersion);
        }

        /// <summary>
        /// Checks whether all the tasks in the specified version have been run.
        /// </summary>
        /// <param name="version">The database version.</param>
        /// <param name="installedVersion">The current installed version.</param>
        /// <returns>true if all the tasks have been installed, otherwise false.</returns>
        private bool CheckTasks(IDatabaseVersion version, VersionBase installedVersion)
        {
            bool hasOutputVersion = false;
            bool allTasksInstalled = true;

            foreach (var task in version.Tasks)
            {
                if (!this.VersionProvider.HasExecutedScript(installedVersion, version.Version, task))
                {
                    if (!hasOutputVersion)
                    {
                        this.MessageService.WriteLine(string.Format("Version {0} missing:", version.Version.VersionText));
                        hasOutputVersion = true;
                    }

                    this.MessageService.WriteLine("  " + task.FileName);
                    allTasksInstalled = false;
                }
            }

            return allTasksInstalled;
        }
    }
}
