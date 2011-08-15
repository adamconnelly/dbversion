using NHibernate;
namespace DatabaseVersion.Tasks
{
    using System.Data;

    /// <summary>
    /// An object that executes tasks.
    /// </summary>
    public interface ITaskExecuter
    {
        /// <summary>
        /// Adds a task to be executed.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        void AddTask(IDatabaseTask task);

        /// <summary>
        /// Executes the tasks.
        /// </summary>
        void ExecuteTasks(ISession session);

        /// <summary>
        /// Returns whether there are any tasks to execute
        /// </summary>
        bool HasTasks { get; }
    }
}
