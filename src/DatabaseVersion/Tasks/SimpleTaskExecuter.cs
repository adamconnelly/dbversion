using System.Collections.Generic;
using System.Data;
using NHibernate;
namespace dbversion.Tasks
{
    public class SimpleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public IMessageService MessageService;

        public SimpleTaskExecuter(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public void ExecuteTasks(ISession session)
        {
            foreach (IDatabaseTask task in this.tasks)
            {
                task.Execute(session, MessageService);
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
