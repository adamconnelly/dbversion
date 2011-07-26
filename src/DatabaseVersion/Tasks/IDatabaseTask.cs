using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DatabaseVersion.Tasks
{
    public interface IDatabaseTask
    {
        int ExecutionOrder { get; }

        /// <summary>
        /// Gets a description of the task.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the filename (if applicable) of the task.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="connection">The connection to use to execute the task.</param>
        /// <exception cref="TaskExecutionException">Thrown if the task fails to execute correctly.</exception>
        void Execute(IDbConnection connection);
    }
}
