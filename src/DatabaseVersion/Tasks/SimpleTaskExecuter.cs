using System.Collections.Generic;
using System.Data;
using NHibernate;
namespace DatabaseVersion.Tasks
{
    public class SimpleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public void ExecuteTasks(ISession session)
        {
            foreach (IDatabaseTask task in this.tasks)
            {
                task.Execute(session);
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
