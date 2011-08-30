using System;
using dbversion.Tasks;

namespace dbversion.Version
{
    public abstract class VersionBase
    {
        public virtual Guid Id { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual DateTime? CreatedOn { get; set; }

        public abstract void AddTask(IDatabaseTask task);

        public abstract bool HasExecutedTask(IDatabaseTask task);
    }
}
