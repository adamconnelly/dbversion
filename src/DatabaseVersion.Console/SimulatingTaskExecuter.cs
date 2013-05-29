namespace dbversion.Console
{
    using System;
    using NHibernate;
    using Tasks;

    /// <summary>
    /// A task executer that just outputs what tasks would be executed instead of actually executing them.
    /// </summary>
    public class SimulatingTaskExecuter : SimpleTaskExecuter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatingTaskExecuter"/> class.
        /// </summary>
        /// <param name="messageService">
        /// The message service.
        /// </param>
        public SimulatingTaskExecuter(IMessageService messageService) : base(messageService)
        {
        }

        /// <summary>
        /// Executes the tasks.
        /// </summary>
        public override void ExecuteTasks(ISession session)
        {
            this.MessageService.WriteLine("Simulating a database update. The following tasks would be performed:" + Environment.NewLine);

            foreach (var task in this.Tasks)
            {
                this.MessageService.WriteLine("  " + task.Description);
            }
        }
    }
}
