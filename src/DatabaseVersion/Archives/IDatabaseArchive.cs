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

        /// <summary>
        /// Checks whether the archive contains the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>true if the archive contains the version, false otherwise.</returns>
        bool ContainsVersion(object version);

        /// <summary>
        /// Returns the correct path to the a script based on the location of the manifest and script
        /// </summary>
        /// <param name="manifestPath">The full path to the location of the manifest</param>
        /// <param name="scriptFileName">The filename of the script</param>
        /// <returns>The correct path for the script</returns>
        string GetScriptPath(string manifestPath, string scriptFileName);
    }
}
