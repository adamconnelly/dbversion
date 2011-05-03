using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion
{
    public class DatabaseVersion : IDatabaseVersion
    {
        public DatabaseVersion(string version, string manifestPath, IDatabaseArchive archive)
        {
            this.Version = version;
            this.ManifestPath = manifestPath;
            this.Archive = archive;
        }

        public string Version { get; private set; }

        public string ManifestPath { get; private set; }

        public IDatabaseArchive Archive { get; private set; }

        public IEnumerable<IDatabaseTask> Tasks { get; set; }
    }
}
