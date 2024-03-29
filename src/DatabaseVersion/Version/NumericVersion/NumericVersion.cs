using System;
using System.Collections.Generic;
using System.Linq;
using dbversion.Tasks;

namespace dbversion.Version.NumericVersion
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
        }

        public NumericVersion()
        {
        }

        public virtual int Version { get; set; }

        public override string VersionText
        {
            get
            {
                return this.Version.ToString();
            }
        }

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

        public override void AddTask(IDatabaseTask task)
        {
            NumericVersionTask script = new NumericVersionTask(this, task.FileName);
            script.ExecutionOrder = task.ExecutionOrder;
            this.Tasks.Add(script);
        }

        public override bool HasExecutedTask(IDatabaseTask task)
        {
            return this.Tasks.Contains(new NumericVersionTask(this, task.FileName));
        }
    }
}