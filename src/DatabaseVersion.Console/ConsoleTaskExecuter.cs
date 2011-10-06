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
                MessageService.WriteLine(String.Format("Started Task {0} of {1}", i, count));
                task.Execute(session, MessageService);
                MessageService.WriteLine(String.Format("Finished Task {0} of {1}, {2:0%} complete", i, count, i / count));
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
