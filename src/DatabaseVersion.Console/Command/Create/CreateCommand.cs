namespace dbversion.Console.Command.Create
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using dbversion.Archives;
    using dbversion.Version;
    using dbversion.Tasks;

    [Export(typeof(IConsoleCommand))]
    public class CreateCommand : ConnectionCommandBase<CreateArguments>
    {
        public override string Name
        {
            get
            {
                return "create";
            }
        }

        public override string Description
        {
            get
            {
                return "Creates or upgrades a database using the specified archive.";
            }
        }

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
                    new CommandParameter("-s", "--saved-connection", "The name of the saved connection to use."),
                    new CommandParameter(null, "--simulate", "Indicates that the update should be simulated and no actual changes should be made."),
                    new CommandParameter("-m", "--missing", "Indicates that any missing tasks should be executed. If the -v flag is used, only that version will be checked."), 
                    new CommandParameter("-r", "--rollback", "Indicates that any changes made by the command should be rolled back.")
                };
            }
        }

        [Import]
        public IDatabaseCreator Creator
        {
            get;
            set;
        }

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
        /// </param>
        /// <param name="arguments">
        /// The parsed arguments.
        /// </param>
        /// <param name="archive">
        /// The database archive containing the versions.
        /// </param>
        /// <returns>Returns the result of Executing the Command</returns>
        protected override bool Execute(string[] args, CreateArguments arguments, IDatabaseArchive archive)
        {
            if (string.IsNullOrEmpty(arguments.Archive))
            {
                this.MessageService.WriteLine("Please specify an archive using the -a switch");
                return false;
            }

            if (archive == null)
            {
                this.MessageService.WriteLine("The specified archive is not supported");
                return false;
            }

            try
            {
                return this.Creator.Create(archive, arguments.Version, CreateTaskExecuter(arguments),
                                           !arguments.ShouldRollbackChanges && !arguments.IsSimulatingUpdate,
                                           arguments.ShouldExecuteMissingTasks);
            }
            catch (VersionNotFoundException v)
            {
                this.MessageService.WriteLine(v.Message);
            }
            catch (TaskExecutionException t)
            {
                this.MessageService.WriteLine(t.Message);
            }
            return false;
        }

        private ITaskExecuter CreateTaskExecuter(CreateArguments arguments)
        {
            if (arguments.IsSimulatingUpdate)
            {
                return new SimulatingTaskExecuter(this.MessageService);
            }

            return new ConsoleTaskExecuter(MessageService);
        }
    }
}
