using System;
using System.Collections.Generic;

namespace DatabaseVersion.Version.ClassicVersion
{
    public class ClassicVersionScript: Script
    {
        public virtual ClassicVersion Version { get; set; }

        public ClassicVersionScript()
        {

        }
        
        public ClassicVersionScript(ClassicVersion version, string scriptName)
            : base(scriptName)
        {
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            ClassicVersionScript version = obj as ClassicVersionScript;
            if (object.ReferenceEquals(version, null))
            {
                return false;
            }

            return (this.Version.Equals(version.Version) &&
                    this.Name.Equals(version.Name, StringComparison.InvariantCultureIgnoreCase));
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
