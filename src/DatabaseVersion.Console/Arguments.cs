using CommandLine.OptParse;
using System.ComponentModel;
using System;
namespace DatabaseVersion.Console
{
    /// <summary>
    /// The command line arguments that define how the application should function.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// The connection string to the database.
        /// </summary>
        [ShortOptionName('c')]
        [OptDef(OptValType.ValueReq)]
        [Description("The connection string for the database.")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// The type of database to connect to.
        /// </summary>
        [ShortOptionName('t')]
        [OptDef(OptValType.ValueReq)]
        [Description("The type of database to connect to.")]
        public string ConnectionType { get; set; }

        /// <summary>
        /// Lists the available connection types.
        /// </summary>
        [OptDef(OptValType.Flag)]
        [Description("Lists the available connection types.")]
        public bool ListConnectionTypes { get; set; }

        /// <summary>
        /// The archive containing the database scripts.
        /// </summary>
        [ShortOptionName('a')]
        [OptDef(OptValType.ValueReq)]
        [Description("The path to the database archive.")]
        public string Archive { get; set; }

        /// <summary>
        /// The version to create or upgrade to.
        /// </summary>
        [ShortOptionName('v')]
        [OptDef(OptValType.ValueOpt)]
        [Description("The version to create or upgrade to. If not specified the latest version is used.")]
        public string Version { get; set; }

        /// <summary>
        /// The mode to use.
        /// </summary>
        public CreationMode CreationMode { get; set; }

        /// <summary>
        /// The string value of the CreationMode argument.
        /// </summary>
        [ShortOptionName('m')]
        [OptDef(OptValType.ValueReq)]
        [Description("Whether to create a new database or upgrade an existing one.")]
        public string CreationModeString
        {
            get
            {
                return this.CreationMode.ToString();
            }

            set
            {
                CreationMode mode;

                Enum.TryParse<CreationMode>(value, out mode);

                this.CreationMode = mode;
            }
        }
    }
}
