using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseVersion.Archives;

namespace DatabaseVersion
{
    public class DatabaseVersion : IDatabaseVersion
    {
        public DatabaseVersion(object version, string manifestPath, IDatabaseArchive archive)
        {
            this.Version = version;
            this.ManifestPath = manifestPath;
            this.Archive = archive;
        }

        public object Version { get; private set; }

        public string ManifestPath { get; private set; }

        public IDatabaseArchive Archive { get; private set; }

        public IEnumerable<IDatabaseTask> Tasks { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [Version: {1}]", this.ManifestPath, this.Version);
        }
    }
}
