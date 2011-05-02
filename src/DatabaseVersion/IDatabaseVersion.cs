using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion
{
    public interface IDatabaseVersion
    {
        string Version { get; }

        /// <summary>
        /// Gets the path to the manifest file.
        /// </summary>
        string ManifestPath { get; }

        IEnumerable<IDatabaseTask> Tasks { get; }
    }
}
