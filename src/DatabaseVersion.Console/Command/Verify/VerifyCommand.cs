using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using dbversion.Console.Command.Create;

namespace dbversion.Console.Command.Verify
{
    [Export(typeof(IConsoleCommand))]
    public class VerifyCommand: CreateCommand
    {
        public override string Name
        {
            get
            {
                return "verify";
            }
        }

        public override string Description
        {
            get
            {
                return "Verify a database creation or upgrade using the specified archive.";
            }
        }

        public override string Usage
        {
            get
            {
                return "dbversion " + this.Name + " [options]";
            }
        }

        protected override bool Commit()
        {
            return false;
        }
    }
}
