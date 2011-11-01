using System;
using System.Collections.Generic;

namespace dbversion.Version.ClassicVersion
{
    public class ClassicVersionTask: Task
    {
        public virtual ClassicVersion Version { get; set; }

        public ClassicVersionTask()
        {

        }
        
        public ClassicVersionTask(ClassicVersion version, string scriptName)
            : base(scriptName)
        {
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            ClassicVersionTask version = obj as ClassicVersionTask;
            if (object.ReferenceEquals(version, null))
            {
                return false;
            }

            return (this.Version.Equals(version.Version) &&
                    this.Name.Equals(version.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return this.Version.GetHashCode() ^ this.Name.GetHashCode();
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
