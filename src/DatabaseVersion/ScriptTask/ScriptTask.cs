using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion.ScriptTask
{
    public class ScriptTask : IDatabaseTask
    {
        public int ExecutionOrder
        {
            get { throw new NotImplementedException(); }
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
