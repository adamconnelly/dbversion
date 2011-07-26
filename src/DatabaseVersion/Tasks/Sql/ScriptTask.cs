using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DatabaseVersion.Archives.File;
using DatabaseVersion.Archives.Zip;

namespace DatabaseVersion.Tasks.Sql
{
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

        public void Execute(IDbConnection connection)
        {
            string filePath = this.GetScriptPath();

            Stream fileStream = this.version.Archive.GetFile(filePath);
            if (fileStream == null)
            {
                throw new TaskExecutionException(string.Format("The script file \"{0}\" does not exist in the archive.", filePath));
            }

            this.ExecuteScript(connection, filePath, fileStream);
        }

        private string GetScriptPath()
        {
            //TODO: Fix this. It's not the best having to check for the type of archive
            if (this.version.Archive is ZipDatabaseArchive)
            {
                //Zip File                
                string filePath = Path.Combine(Path.GetDirectoryName(this.version.ManifestPath), this.FileName);
                return filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            else
            {
                FileInfo manifestFile = new FileInfo(this.version.ManifestPath);
                string filePath = Path.Combine(manifestFile.Directory.Name, this.FileName);
                return filePath;
            }
        }

        private void ExecuteScript(IDbConnection connection, string filePath, Stream fileStream)
        {
            //TODO: Shouldn't need to do this as we should already be at the beginning
            fileStream.Position = 0;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                IEnumerable<string> batches = GetQueryBatches(reader.ReadToEnd());
                foreach (string batch in batches)
                {
                    try
                    {
                        this.ExecuteQueryBatch(batch, connection);
                    }
                    catch (Exception e)
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

        private void ExecuteQueryBatch(string batch, IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
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
