using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DatabaseVersion.ScriptTask
{
    public class ScriptTaskFactory : IDatabaseTaskFactory
    {
        public bool CanHandle(XElement element)
        {
            return element != null && element.Name == "script";
        }

        public IDatabaseTask Create(XElement element)
        {
            return new ScriptTask();
        }
    }
}
