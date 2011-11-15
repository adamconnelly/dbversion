namespace dbversion.Console.Command.History
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using CommandLine;

    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Settings;
    using dbversion.Version;

    using Session = dbversion.Session;

    /// <summary>
    /// A command for outputting the history of versions installed in the database.
    /// </summary>
    [Export(typeof(IConsoleCommand))]
    public class HistoryCommand : ConnectionCommandBase<HistoryArguments>
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "history";
            }
        }

        /// <summary>
        /// Gets a description of the command.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Prints out a history of the installed versions.";
            }
        }

        /// <summary>
        /// Gets the usage of the command.
        /// </summary>
        public override string Usage
        {
            get
            {
                return string.Format("dbversion {0} [options]", this.Name);
            }
        }

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        public override IEnumerable<CommandParameter> Parameters
        {
            get
            {
                return new[]
                {
                    new CommandParameter("-c", "--connectionString", "The database connection string."),
                    new CommandParameter("-p", "--connectionProvider", "The hibernate connection provider."),
                    new CommandParameter("-d", "--driverClass", "The hibernate driver class."),
                    new CommandParameter("-l", "--dialect", "The hibernate dialect."),
                    new CommandParameter("-o", "--order", "The order to sort the versions by [asc|desc]."),
                    new CommandParameter("-t", "--showTasks", "Indicates whether the tasks for each version should be output."),
                    new CommandParameter("-s", "--saved-connection", "The name of the saved connection to use.")
                };
            }
        }

        /// <summary>
        /// Gets or sets the session factory provider.
        /// </summary>
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider { get; set; }

        /// <summary>
        /// Gets or sets the version provider.
        /// </summary>
        [Import]
        public IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        protected override void Execute (string[] args, HistoryArguments arguments, dbversion.Archives.IDatabaseArchive archive)
        {
            VersionBase requiredVersion = null;
            if (!string.IsNullOrEmpty(arguments.Version))
            {
                requiredVersion = this.VersionProvider.CreateVersion(arguments.Version);
            }

            using (var sessionFactory = this.SessionFactoryProvider.CreateSessionFactory())
            {
                using (var session = sessionFactory.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        if (!this.VersionProvider.VersionTableExists(session))
                        {
                            this.MessageService.WriteLine("No versions are currently installed.");
                            return;
                        }

                        var sortedVersions = this.GetSortedVersions(arguments, requiredVersion, session);

                        if (sortedVersions.Count() == 0 && requiredVersion != null)
                        {
                            this.MessageService.WriteLine(
                                string.Format("The specified version, {0}, was not found.", arguments.Version));
                            return;
                        }

                        foreach (var version in sortedVersions)
                        {
                            this.WriteVersion(version, arguments);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the versions sorted in the correct order.
        /// </summary>
        /// <returns>
        /// The sorted versions.
        /// </returns>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        /// <param name='session'>
        /// The NHibernate session.
        /// </param>
        private IEnumerable<VersionBase> GetSortedVersions(HistoryArguments arguments, VersionBase version, NHibernate.ISession session)
        {
            var versions = this.VersionProvider.GetAllVersions(session);
            if (arguments.SortOrder == HistoryOrder.asc || arguments.SortOrder == HistoryOrder.Ascending)
            {
                versions = versions.OrderBy(v => v, this.VersionProvider.GetComparer());
            }
            else
            {
                versions = versions.OrderByDescending(v => v, this.VersionProvider.GetComparer());
            }

            if (version != null)
            {
                return versions.Where(v => object.Equals(v, version));
            }

            return versions;
        }

        /// <summary>
        /// Writes the specified version.
        /// </summary>
        /// <param name='version'>
        /// The version to write.
        /// </param>
        /// <param name='arguments'>
        /// The command arguments.
        /// </param>
        private void WriteVersion (VersionBase version, HistoryArguments arguments)
        {
            string versionString = string.Format(
                "{0} Installed - {1}, Updated - {2}",
                version.VersionText,
                version.CreatedOnLocal,
                version.UpdatedOnLocal);

            this.MessageService.WriteLine(versionString);

            if (arguments.ShowTasks)
            {
                this.WriteTasks(version, arguments.SortOrder);
            }
        }

        /// <summary>
        /// Writes the tasks.
        /// </summary>
        /// <param name='version'>
        /// The version containing the tasks.
        /// </param>
        /// <param name='sortOrder'>
        /// The order to write the tasks in.
        /// </param>
        private void WriteTasks(VersionBase version, HistoryOrder sortOrder)
        {
            IEnumerable<Task> sortedTasks;
            if (sortOrder == HistoryOrder.asc || sortOrder == HistoryOrder.Ascending)
            {
                sortedTasks = version.Tasks.OrderBy(t => t.ExecutionOrder);
            }
            else
            {
                sortedTasks = version.Tasks.OrderByDescending(t => t.ExecutionOrder);
            }

            foreach (var task in sortedTasks)
            {
                string taskString = string.Format(
                    "  {0} - {1}",
                    task.Name,
                    task.UpdatedOnLocal);

                this.MessageService.WriteLine(taskString);
            }
        }
    }
}

