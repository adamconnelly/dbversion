using CommandLine.OptParse;
using System.ComponentModel;
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
        [ShortOptionName('m')]
        [OptDef(OptValType.ValueReq)]
        [Description("Whether to create a new database or upgrade an existing one.")]
        public CreationMode CreationMode { get; set; }
    }
}
