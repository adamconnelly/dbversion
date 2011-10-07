namespace dbversion.Tasks.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.IO;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    using dbversion.Version;

    using NHibernate;

    public class ScriptTask : IDatabaseTask, IEqualityComparer<ScriptTask>
    {
        /// <summary>
        /// The string that separates the script into batches.
        /// </summary>
        private const string BatchSeparator = "go";

        /// <summary>
        /// The regex to use to split the script into batches.
        /// </summary>
        private const string SeparatorRegexFormat =
            "{0}{1}|{0}{1}{0}|{1}{0}";

        /// <summary>
        /// The regex object created from the regex format.
        /// </summary>
        private readonly Regex StringSplitRegex;

        /// <summary>
        /// The database version that specifies that the script should be run.
        /// </summary>
        private readonly IDatabaseVersion version;

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
                return string.Format("Executing script \"{0}\"", this.GetScriptPath());
            }
        }

        /// <summary>
        /// The MessageService to use for logging
        /// </summary>
        public IMessageService MessageService
        {
            get;
            set;
        }


        public ScriptTask(string fileName, int executionOrder, IDatabaseVersion version)
        {
            this.FileName = fileName;
            this.ExecutionOrder = executionOrder;
            this.version = version;

            this.StringSplitRegex = GetStringSplitRegex();
        }

        private static Regex GetStringSplitRegex()
        {
            return new Regex(string.Format(SeparatorRegexFormat, Environment.NewLine, BatchSeparator), RegexOptions.IgnoreCase);
        }

        #region Logging

        private DateTime _taskStartTime;
        private DateTime _batchStartTime;

        private void LogTaskStart(int taskNumber, int totalTasks)
        {
            _taskStartTime = DateTime.Now;
            MessageService.WriteLine(String.Format("Starting Task {0} of {1}: {2}", taskNumber, totalTasks, Description));
        }

        private void LogTaskStop(int taskNumber, int totalTasks)
        {
            MessageService.WriteLine(String.Format("Finished Task {0} of {1}: {2}. Time Taken: {3}, {4:0%} complete",
                                       taskNumber, totalTasks, Description, DateTime.Now.Subtract(_taskStartTime),
                                       taskNumber / totalTasks)); 
        }

        private void LogBatchStart(int batchNumber, int totalBatches)
        {
            _batchStartTime = DateTime.Now;
            MessageService.WriteLine(String.Format("Starting Batch {0} of {1}", batchNumber, totalBatches));
        }

        private void LogBatchStop(int batchNumber, int totalBatches)
        {
            MessageService.WriteLine(String.Format("Finished Batch {0} of {1}. Time Taken: {2}", batchNumber, totalBatches, DateTime.Now.Subtract(_batchStartTime)));
        }

        #endregion

        public void Execute(ISession session, IMessageService messageService, int taskNumber, int totalTasks)
        {
            MessageService = messageService;

            LogTaskStart(taskNumber, totalTasks);

            string filePath = this.GetScriptPath();

            Stream fileStream = this.version.Archive.GetFile(filePath);
            if (fileStream == null)
            {
                string message = string.Format("The script file \"{0}\" does not exist in the archive.", filePath);
                messageService.WriteLine(message);

                throw new TaskExecutionException(message);
            }

            this.ExecuteScript(session, filePath, fileStream);

            LogTaskStop(taskNumber, totalTasks);
        }

        private string GetScriptPath()
        {
            return this.version.Archive.GetScriptPath(this.version.ManifestPath, this.FileName);
        }

        private void ExecuteScript(ISession session, string filePath, Stream fileStream)
        {
            //TODO: Shouldn't need to do this as we should already be at the beginning
            fileStream.Position = 0;

            using (StreamReader reader = new StreamReader(fileStream, Encoding.Default, true))
            {
                IEnumerable<string> batches = GetQueryBatches(reader.ReadToEnd());
                int i = 0, count = batches.Count();
                foreach (string batch in batches)
                {
                    i++;
                    try
                    {
                        LogBatchStart(i, count);
                        this.ExecuteQueryBatch(batch, session);
                        LogBatchStop(i, count);
                    }
                    catch (Exception e)
                    {
                        string exceptionMessage = string.Format("Failed to execute Batch {0} of script \"{1}\". {2}", i, filePath, e.Message);
                        MessageService.WriteExceptionLine(exceptionMessage, e);

                        throw new TaskExecutionException(exceptionMessage, e);
                    }
                }
            }
        }

        private IEnumerable<string> GetQueryBatches(string scriptContents)
        {
            return this.StringSplitRegex
                .Split(scriptContents)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b.Trim());
        }

        private void ExecuteQueryBatch(string batch, ISession session)
        {
            MessageService.WriteDebugLine(String.Format("{0}", batch));

            var query = session.CreateSQLQuery(batch);
            query.ExecuteUpdate();
        }

        public bool Equals(ScriptTask x, ScriptTask y)
        {
            return x.FileName.Equals(y.FileName, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ScriptTask obj)
        {
            return obj.FileName.GetHashCode();
        }
    }
}