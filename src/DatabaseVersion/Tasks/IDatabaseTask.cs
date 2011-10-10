using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NHibernate;

namespace dbversion.Tasks
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
        /// <param name="session">The connection to use to execute the task.</param>
        /// <param name="taskNumber">The number of the task with the collection of tasks</param>
        /// <param name="totalTasks">The total number of tasks to be executed</param>
        /// <exception cref="TaskExecutionException">Thrown if the task fails to execute correctly.</exception>
        void Execute(ISession session, int taskNumber, int totalTasks);
    }
}
