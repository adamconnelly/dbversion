using System.Collections.Generic;
using System.Data;
using NHibernate;
namespace dbversion.Tasks
{
    public class SimpleTaskExecuter : ITaskExecuter
    {
        private readonly Queue<IDatabaseTask> tasks = new Queue<IDatabaseTask>();

        public IMessageService MessageService;

        protected Queue<IDatabaseTask> Tasks
        {
            get { return this.tasks; }
        }

        public SimpleTaskExecuter(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public void AddTask(IDatabaseTask task)
        {
            this.tasks.Enqueue(task);
        }

        public virtual void ExecuteTasks(ISession session)
        {
            int count = tasks.Count;
            for (int i = 1; i < count + 1; i++)
            {
                IDatabaseTask task = this.tasks.Dequeue();
                task.Execute(session, i, count);
            }
        }

        public bool HasTasks { get { return tasks.Count > 0; } }
    }
}
