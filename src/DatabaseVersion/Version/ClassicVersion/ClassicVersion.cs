using System.Collections.Generic;
using System.Linq;
using dbversion.Tasks;

namespace dbversion.Version.ClassicVersion
{
    public class ClassicVersion : VersionBase
    {
        public virtual IEnumerable<ClassicVersionTask> Tasks { get; set; }

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
            this.Tasks = new List<ClassicVersionTask>();
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
            
            return string.Format("{0} - {1}", this.Version, this.UpdatedOnLocal);
        }

        public override void AddTask(IDatabaseTask task)
        {
            ClassicVersionTask script = new ClassicVersionTask(this, task.FileName);
            script.ExecutionOrder = task.ExecutionOrder;
            (this.Tasks as IList<ClassicVersionTask>).Add(script);
        }

        public override bool HasExecutedTask(IDatabaseTask task)
        {
            return this.Tasks.Contains(new ClassicVersionTask(this, task.FileName));
        }
    }
}