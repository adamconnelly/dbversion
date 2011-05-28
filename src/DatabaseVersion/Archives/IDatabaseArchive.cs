using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DatabaseVersion.Archives
{
    /// <summary>
    /// An archive containing a collection of database versions.
    /// </summary>
    public interface IDatabaseArchive
    {
        /// <summary>
        /// Gets the path to the archive.
        /// </summary>
        string ArchivePath { get; }

        /// <summary>
        /// Gets the database versions present in the archive.
        /// </summary>
        IEnumerable<IDatabaseVersion> Versions { get; }

        /// <summary>
        /// Gets the file at the specified path.
        /// </summary>
        /// <param name="path">The path relative to the root of the archive.</param>
        /// <returns>The file stream or null if the file does not exist.</returns>
        Stream GetFile(string path);
    }
}
