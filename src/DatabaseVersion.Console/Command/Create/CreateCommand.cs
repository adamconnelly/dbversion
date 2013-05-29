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
                    new CommandParameter("-s", "--saved-connection", "The name of the saved connection to use.")
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
        /// If true, commit the overall transaction. By returning false, the effect of the upgrade can be 'tested'
        /// </summary>
        protected virtual bool Commit()
        {
            return true;
        }

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name='args'>
        /// The arguments.
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
                return this.Creator.Create(archive, arguments.Version, new ConsoleTaskExecuter(MessageService), Commit());
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
    }
}
