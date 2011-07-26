using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion.Version.NumericVersion
{
    public class NumericVersionScript : Script
    {
        public virtual NumericVersion Version { get; set; }

        public NumericVersionScript()
        {

        }

        public NumericVersionScript(NumericVersion version, string scriptName)
            : base(scriptName)
        {
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            NumericVersionScript script = obj as NumericVersionScript;
            if (object.ReferenceEquals(script, null))
            {
                return false;
            }

            return (this.Version.Equals(script.Version) &&
                    this.Name.Equals(script.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return this.Version.GetHashCode();
        }

        public override string ToString()
        {
            if (UpdatedOn == null)
            {
                return this.Version.ToString();
            }

            return string.Format("{0} - {1}", this.Version, this.UpdatedOn);
        }
    }
}
