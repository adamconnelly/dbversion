using System.Collections.Generic;
using System.Data;
namespace DatabaseVersion.Tasks
{
    public class SimpleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public void ExecuteTasks(IDbConnection connection)
        {
            foreach (IDatabaseTask task in this.tasks)
            {
                task.Execute(connection);
            }
        }
    }
}
