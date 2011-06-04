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
        public Arguments()
        {
            this.PluginDirectory = "plugins";
        }

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
        [UseNameAsLongOption(false)]
        [LongOptionName("listConnectionTypes")]
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
        /// Gets or sets the directory that plugins will be loaded from.
        /// </summary>
        [ShortOptionName('p')]
        [OptDef(OptValType.ValueOpt)]
        [Description("The directory that plugins will be loaded from.")]
        [DefaultValue("plugins")]
        public string PluginDirectory
        {
            get;
            set;
        }
    }
}
