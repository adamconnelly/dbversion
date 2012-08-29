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

    using dbversion.Property;
    using dbversion.Version;

    using NHibernate;

    public class ScriptTask : BaseTask, IEqualityComparer<ScriptTask>
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

        private readonly IPropertyService propertyService;

        protected override string GetTaskDescription()
        {
            return string.Format("Executing script \"{0}\"", this.GetScriptPath());
        }

        /// <summary>
        /// The Timeout in seconds for the task
        /// </summary>
        /// <returns>The Timeout in seconds or if none set null</returns>
        public int? TaskTimeout
        {
            get
            {
                var property = this.propertyService["dbversion.sql.command_timeout"];
                if (property != null)
                {
                    int commandTimeout;
                    if (Int32.TryParse(property.Value, out commandTimeout))
                        return commandTimeout;
                }

                return null;
            }
        }

        public ScriptTask(string fileName, int executionOrder, IDatabaseVersion version, IMessageService messageService, IPropertyService propertyService)
            : base(fileName, executionOrder, messageService)
        {
            this.version = version;
            this.propertyService = propertyService;

            this.StringSplitRegex = GetStringSplitRegex();
        }

        private static Regex GetStringSplitRegex()
        {
            return new Regex(string.Format(SeparatorRegexFormat, Environment.NewLine, BatchSeparator), RegexOptions.IgnoreCase);
        }

        protected override void ExecuteTask(ISession session)
        {
            string filePath = this.GetScriptPath();

            Stream fileStream = this.version.Archive.GetFile(filePath);
            if (fileStream == null)
            {
                string message = string.Format("The script file \"{0}\" does not exist in the archive.", filePath);
                MessageService.WriteLine(message);

                throw new TaskExecutionException(message);
            }

            this.ExecuteScript(session, filePath, fileStream);
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

            using (var command = session.Connection.CreateCommand())
            {
                session.Transaction.Enlist(command);
                if (TaskTimeout.HasValue) command.CommandTimeout = TaskTimeout.Value;
                command.CommandText = batch;
                command.ExecuteNonQuery();
            }
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