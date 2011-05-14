using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseVersion.Archives;
using System.IO;

namespace DatabaseVersion.Manifests
{
    /// <summary>
    /// An object that can read database manifests to produce <see cref="IDatabaseVersion"/> instances.
    /// </summary>
    public interface IManifestReader
    {
        /// <summary>
        /// Reads the specified manifest to produce an <see cref="IDatabaseVersion"/> instance.
        /// </summary>
        /// <param name="stream">The stream containing the manifest.</param>
        /// <param name="manifestPath">The path to the manifest.</param>
        /// <param name="archive">The archive containing the manifest.</param>
        /// <returns>The manifest.</returns>
        IDatabaseVersion Read(Stream stream, string manifestPath, IDatabaseArchive archive);
    }
}
