using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;

namespace DatabaseVersion.Sql
{
    public class ScriptTask : IDatabaseTask
    {
        private readonly IDatabaseVersion version;

        public ScriptTask(string fileName, int executionOrder, IDatabaseVersion version)
        {
            this.FileName = fileName;
            this.ExecutionOrder = executionOrder;
            this.version = version;
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

        public void Execute(IDbConnection connection)
        {
            FileInfo manifestFile = new FileInfo(this.version.ManifestPath);
            
            Stream fileStream = this.version.Archive.GetFile(Path.Combine(manifestFile.Directory.Name, this.FileName));
            using (StreamReader reader = new StreamReader(fileStream))
            {
                IEnumerable<string> batches = GetQueryBatches(reader.ReadToEnd());
                foreach (string batch in batches)
                {
                    ExecuteQueryBatch(batch, connection);
                }
            }
        }

        private IEnumerable<string> GetQueryBatches(string scriptContents)
        {
            return scriptContents.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        }

        private void ExecuteQueryBatch(string batch, IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = batch;
                command.ExecuteNonQuery();
            }
        }
    }
}
