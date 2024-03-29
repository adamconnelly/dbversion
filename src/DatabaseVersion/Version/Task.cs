using System;
using System.Collections.Generic;

namespace dbversion.Version
{
    public abstract class Task
    {
        #region Properties

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }

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

        public virtual int ExecutionOrder { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptName"></param>
        public Task(string scriptName)
        {
            this.Name = scriptName;
            this.UpdatedOn = DateTime.UtcNow;
        }

        protected Task()
            : this(String.Empty)
        {

        }

        #endregion
    }
}
