namespace DatabaseVersion.Console
{
    using System;
    using System.Collections.Generic;

    using Tasks;

    public class ConsoleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public void ExecuteTasks(System.Data.IDbConnection connection)
        {
            double count = this.tasks.Count;
            for (int i = 1; i < count + 1; i++)
            {
                IDatabaseTask task = this.tasks.Dequeue();
                task.Execute(connection);
                Console.WriteLine("{0:0%} complete. {1}.", i / count, task.Description);
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
