using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseVersion.Archives;

namespace DatabaseVersion
{
    public interface IDatabaseVersion
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the path to the manifest file.
        /// </summary>
        string ManifestPath { get; }

        /// <summary>
        /// Gets the archive containing the database version.
        /// </summary>
        IDatabaseArchive Archive { get; }

        /// <summary>
        /// Gets the tasks required to create the database version.
        /// </summary>
        IEnumerable<IDatabaseTask> Tasks { get; }
    }
}
