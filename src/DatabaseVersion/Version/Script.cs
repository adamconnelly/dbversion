using System;
using System.Collections.Generic;

namespace DatabaseVersion.Version
{
    public abstract class Script
    {
        #region Properties

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int ScriptOrder { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptName"></param>
        public Script(string scriptName)
        {
            this.Name = scriptName;
            this.UpdatedOn = DateTime.UtcNow;
        }

        protected Script()
            : this(String.Empty)
        {

        }

        #endregion
    }
}
