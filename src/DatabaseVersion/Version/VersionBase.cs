using System;
using dbversion.Tasks;

namespace dbversion.Version
{
    public abstract class VersionBase
    {
        public virtual Guid Id { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        
        /// <summary>
        /// Gets the time the version was last updated in local time.
        /// </summary>
        public virtual DateTime? UpdatedOnLocal
        {
            get
            {
                if (this.UpdatedOn != null)
                {
                    return this.UpdatedOn.Value.ToLocalTime();
                }

                return null;
            }
        }

        public virtual DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets the time the version was created in local time.
        /// </summary>
        public virtual DateTime? CreatedOnLocal
        {
            get
            {
                if (this.CreatedOn != null)
                {
                    return this.CreatedOn.Value.ToLocalTime();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a textual description of the version.
        /// </summary>
        public abstract string VersionText { get; }

        public abstract void AddTask(IDatabaseTask task);

        public abstract bool HasExecutedTask(IDatabaseTask task);
    }
}
