using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion.Version
{
    public class VersionBase
    {
        public virtual Guid Id { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual DateTime? CreatedOn { get; set; }

        public virtual void AddScript(string scriptName, int executionOrder)
        {

        }

        public virtual bool HasExecutedScript(string scriptName)
        {
            return false;
        }
    }
}
