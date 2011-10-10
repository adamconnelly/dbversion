using System;
using System.IO;
using dbversion.Version;
using NHibernate;

namespace dbversion.Tasks
{
    public abstract class BaseTask: IDatabaseTask
    {
        private DateTime _taskStartTime;
        private DateTime _batchStartTime;

        public BaseTask(string fileName, int executionOrder, IMessageService messageService)
        {
            this.MessageService = messageService;
            this.FileName = fileName;
            this.ExecutionOrder = executionOrder;
        }

        /// <summary>
        /// The FileName of the task to run
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// The Execution Order of this task
        /// </summary>
        public int ExecutionOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// A description of the Task
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetTaskDescription();
            }
        }

        /// <summary>
        /// The MessageService to use for logging
        /// </summary>
        public IMessageService MessageService
        {
            get;
            private set;
        }

        #region Logging 

        protected void LogTaskStart(int taskNumber, int totalTasks)
        {
            _taskStartTime = DateTime.Now;
            MessageService.WriteLine(String.Format("Starting Task {0} of {1}: {2}", taskNumber, totalTasks, Description));
        }

        protected void LogTaskStop(int taskNumber, int totalTasks)
        {
            MessageService.WriteLine(String.Format("Finished Task {0} of {1}: {2}. Time Taken: {3}, {4:0%} complete",
                                                   taskNumber, totalTasks, Description, DateTime.Now.Subtract(_taskStartTime),
                                                   taskNumber / totalTasks)); 
        }

        protected void LogBatchStart(int batchNumber, int totalBatches)
        {
            _batchStartTime = DateTime.Now;
            MessageService.WriteLine(String.Format("Starting Batch {0} of {1}", batchNumber, totalBatches));
        }

        protected void LogBatchStop(int batchNumber, int totalBatches)
        {
            MessageService.WriteLine(String.Format("Finished Batch {0} of {1}. Time Taken: {2}", batchNumber, totalBatches, DateTime.Now.Subtract(_batchStartTime)));
        }

        #endregion

        protected abstract string GetTaskDescription();

        protected abstract void ExecuteTask(ISession session);

        public void Execute(ISession session, int taskNumber, int totalTasks)
        {
            LogTaskStart(taskNumber, totalTasks);

            ExecuteTask(session);

            LogTaskStop(taskNumber, totalTasks);
        }
    }
}