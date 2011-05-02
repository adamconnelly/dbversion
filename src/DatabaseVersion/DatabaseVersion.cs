using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion
{
    public class DatabaseVersion : IDatabaseVersion
    {
        public DatabaseVersion(string version, string manifestPath, IEnumerable<IDatabaseTask> tasks)
        {
            this.Version = version;
            this.ManifestPath = manifestPath;
            this.Tasks = tasks;
        }

        public string Version { get; private set; }

        public string ManifestPath { get; private set; }

        public IEnumerable<IDatabaseTask> Tasks { get; private set; }
    }
}
