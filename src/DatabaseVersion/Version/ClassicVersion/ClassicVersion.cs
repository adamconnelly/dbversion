using System.Collections.Generic;

namespace DatabaseVersion.Version.ClassicVersion
{
    public class ClassicVersion: VersionBase
    {
        public virtual IList<ClassicVersionScript> Scripts { get; set; }

        public virtual string Version
        {
            get { return _version.ToString(); }
            set { _version = new System.Version(value); }
        }

        public virtual System.Version SystemVersion
        {
            get { return _version; }
        }

        private System.Version _version;

        public ClassicVersion()
            : this("1.0")
        {

        }

        public ClassicVersion(string version)
        {
            this.Version = version;
            this.Scripts = new List<ClassicVersionScript>();
        }

        public override bool Equals(object obj)
        {
            ClassicVersion version = obj as ClassicVersion;
            if (object.ReferenceEquals(version, null))
            {
                return false;
            }

            return this.SystemVersion.Equals(version.SystemVersion);
        }

        public override int GetHashCode()
        {
            return this.SystemVersion.GetHashCode();
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
            ClassicVersionScript script = new ClassicVersionScript(this, scriptName);
            script.ScriptOrder = scriptOrder;
            this.Scripts.Add(script);
        }

        public override bool HasExecutedScript(string scriptName)
        {
            return this.Scripts.Contains(new ClassicVersionScript(this, scriptName));
        }
    }
}
