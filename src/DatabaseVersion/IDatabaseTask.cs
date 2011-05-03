using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DatabaseVersion
{
    public interface IDatabaseTask
    {
        int ExecutionOrder { get; }
        void Execute(IDbConnection connection);
    }
}
