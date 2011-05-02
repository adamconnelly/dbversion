using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion
{
    public interface IDatabaseTask
    {
        int ExecutionOrder { get; }
        void Execute();
    }
}
