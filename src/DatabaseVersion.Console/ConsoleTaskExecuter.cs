using NHibernate;
namespace dbversion.Console
{
    using System;
    using System.Collections.Generic;

    using Tasks;

    public class ConsoleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public IMessageService MessageService;

        public ConsoleTaskExecuter(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public void ExecuteTasks(ISession session)
        {
            double count = this.tasks.Count;
            for (int i = 1; i < count + 1; i++)
            {
                IDatabaseTask task = this.tasks.Dequeue();
                DateTime startTime = DateTime.Now;
                MessageService.WriteLine(String.Format("Starting Task {0} of {1}: {2}", i, count, task.Description));
                task.Execute(session, MessageService);
                MessageService.WriteLine(String.Format("Finished Task {0} of {1}: {2}. Time Taken: {3}, {4:0%} complete", i, count, task.Description, DateTime.Now.Subtract(startTime), i / count));
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
