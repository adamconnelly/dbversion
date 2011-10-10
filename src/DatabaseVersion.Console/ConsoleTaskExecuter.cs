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
            int count = this.tasks.Count;
            for (int i = 1; i < count + 1; i++)
            {
                IDatabaseTask task = this.tasks.Dequeue();
                task.Execute(session, i, count);
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
