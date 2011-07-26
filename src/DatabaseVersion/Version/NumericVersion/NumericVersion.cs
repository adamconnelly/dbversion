using System;
using System.Collections.Generic;

namespace DatabaseVersion.Version.NumericVersion
{
    public class NumericVersion: VersionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericVersion"/> class.
        /// </summary>
        /// <param name="version">The version number.</param>
        public NumericVersion(int version)
        {
            this.Version = version;
            this.Scripts = new List<NumericVersionScript>();
        }

        public NumericVersion()
        {
            
        }

        public virtual int Version { get; set; }

        public virtual IList<NumericVersionScript> Scripts { get; set; }

        public override bool Equals(object obj)
        {
            NumericVersion version = obj as NumericVersion;
            if (object.ReferenceEquals(version, null))
            {
                return false;
            }

            return object.Equals(this.Version, version.Version);
        }

        public override int GetHashCode()
        {
            return this.Version;
        }

        public override string ToString()
        {
            if (UpdatedOn == null)
            {
                return this.Version.ToString();
            }

            return string.Format("{0} - {1}", this.Version, this.UpdatedOn);
        }

        public override void AddScript(string scriptName, int scriptOrder)
        {
            NumericVersionScript script = new NumericVersionScript(this, scriptName);
            script.ScriptOrder = scriptOrder;
            this.Scripts.Add(script);
        }

        public override bool HasExecutedScript(string scriptName)
        {
            return this.Scripts.Contains(new NumericVersionScript(this, scriptName));
        }
    }
}
