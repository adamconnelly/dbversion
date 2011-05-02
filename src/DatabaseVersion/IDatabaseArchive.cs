using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DatabaseVersion
{
    public interface IDatabaseArchive
    {
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
