using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion.Archives
{
    /// <summary>
    /// A factory for creating <see cref="IDatabaseArchive"/> objects.
    /// </summary>
    public interface IDatabaseArchiveFactory
    {
        /// <summary>
        /// Returns whether this factory can create an archive from the specified path.
        /// </summary>
        /// <param name="path">The path to create the archive from.</param>
        /// <returns>true if the factory can create an archive, false otherwise.</returns>
        bool CanCreate(string path);

        /// <summary>
        /// Creates an archive from the specified path.
        /// </summary>
        /// <param name="path">The path to create the archive from.</param>
        /// <returns>The archive.</returns>
        IDatabaseArchive Create(string path);
    }
}
