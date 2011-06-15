
using System.ComponentModel;
using System;
using CommandLine;
using CommandLine.Text;
namespace DatabaseVersion.Console
{
    /// <summary>
    /// The command line arguments that define how the application should function.
    /// </summary>
    public class Arguments
    {
        public Arguments()
        {
            this.PluginDirectory = "plugins";
        }

        /// <summary>
        /// The connection string to the database.
        /// </summary>
        [Option("c", "connectionString", HelpText = "The database connection string.")]
        public string ConnectionString;

        /// <summary>
        /// The type of database to connect to.
        /// </summary>
        [Option("t", "connectionType", HelpText = "The type of database you are connecting to.")]
        public string ConnectionType;

        /// <summary>
        /// Lists the available connection types.
        /// </summary>
        [Option(null, "listConnectionTypes", HelpText = "Lists the available connection types.")]
        public bool ListConnectionTypes;

        /// <summary>
        /// The archive containing the database scripts.
        /// </summary>
        [Option("a", "archive", HelpText = "The archive containing the database versions.")]
        public string Archive;

        /// <summary>
        /// The version to create or upgrade to.
        /// </summary>
        [Option("v", "version", HelpText = "The version to create or upgrade to.")]
        public string Version;
        
        /// <summary>
        /// Gets or sets the directory that plugins will be loaded from.
        /// </summary>
        [Option("p", "pluginDir", HelpText = "The directory to load plugins from.")]
        public string PluginDirectory = "plugins";

        [HelpOption("h", "help", HelpText = "Display this help screen.")]
        public string GetHelp()
        {
            var help = new HelpText("dbversion");
            help.AdditionalNewLineAfterOption = true;
            help.AddOptions(this);

            return help;
        }
    }
}
