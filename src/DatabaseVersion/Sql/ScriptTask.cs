using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DatabaseVersion.Sql
{
    public class ScriptTask : IDatabaseTask
    {
        public ScriptTask(string fileName, int executionOrder)
        {
            this.FileName = fileName;
            this.ExecutionOrder = executionOrder;
        }

        public string FileName
        {
            get;
            private set;
        }

        public int ExecutionOrder
        {
            get;
            private set;
        }

        public void Execute(IDbConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
