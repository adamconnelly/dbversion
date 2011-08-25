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

        public ScriptTask(string fileName, int executionOrder, IDatabaseVersion version)
        {
            this.FileName = fileName;
            this.ExecutionOrder = executionOrder;
            this.version = version;

            this.StringSplitRegex = GetStringSplitRegex();
        }

        private Regex GetStringSplitRegex()
        {
            return new Regex(string.Format(SeparatorRegexFormat, Environment.NewLine, BatchSeparator), RegexOptions.IgnoreCase);
        }

        public string FileName
        {
            get;
            private set;
        }

        public int ExecutionOrder
        {
            get;
            private set;
        }

        public string Description
        {
            get
            {
                return string.Format("Executed script \"{0}\"", this.GetScriptPath());
            }
        }

        public void Execute(ISession session)
        {
            string filePath = this.GetScriptPath();

            Stream fileStream = this.version.Archive.GetFile(filePath);
            if (fileStream == null)
            {
                throw new TaskExecutionException(string.Format("The script file \"{0}\" does not exist in the archive.", filePath));
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
                IEnumerable<string > batches = GetQueryBatches(reader.ReadToEnd());
                foreach (string batch in batches)
                {
                    try
                    {
                        this.ExecuteQueryBatch(batch, session);
                    } catch (Exception e)
                    {
                        string exceptionMessage = string.Format("Failed to execute script \"{0}\". {1}", filePath, e.Message);
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