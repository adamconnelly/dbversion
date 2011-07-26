using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel.Composition;
using DatabaseVersion.Manifests;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DatabaseVersion.Archives.File
{
    /// <summary>
    /// A file based database archive.
    /// </summary>
    public class FileDatabaseArchive : IDatabaseArchive
    {
        /// <summary>
        /// The reader used to create the <see cref="IDatabaseVersion"/> objects.
        /// </summary>
        private readonly IManifestReader manifestReader;

        /// <summary>
        /// The backing store for <see cref="Versions"/>.
        /// </summary>
        private ConcurrentBag<IDatabaseVersion> versions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDatabaseArchive"/> class.
        /// </summary>
        /// <param name="archivePath">The root directory of the archive.</param>
        public FileDatabaseArchive(string archivePath, IManifestReader manifestReader)
        {
            this.ArchivePath = archivePath;
            this.manifestReader = manifestReader;
        }

        /// <summary>
        /// Gets the list of database versions from the archive.
        /// </summary>
        public IEnumerable<IDatabaseVersion> Versions
        {
            get
            {
                if (this.versions == null)
                {
                    this.versions = new ConcurrentBag<IDatabaseVersion>();
                    DirectoryInfo info = new DirectoryInfo(this.ArchivePath);
                    var manifests = info.GetFiles("database.xml", SearchOption.AllDirectories);

                    Parallel.ForEach(manifests, ParseManifest);
                }

                return this.versions;
            }
        }

        /// <summary>
        /// The path to the archive.
        /// </summary>
        public string ArchivePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the file at the specified path from the archive.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The stream containing the file contents.</returns>
        public Stream GetFile(string path)
        {
            var fileInfo = new FileInfo(Path.Combine(this.ArchivePath, path));

            if (fileInfo.Exists)
            {
                return fileInfo.Open(FileMode.Open);
            }

            return null;
        }

        /// <summary>
        /// Checks whether the archive contains the specified version.
        /// </summary>
        /// <param name="version">The version to look for.</param>
        /// <returns>true if the version is contained within the archive, false otherwise.</returns>
        public bool ContainsVersion(object version)
        {
            return this.Versions.FirstOrDefault(v => object.Equals(v.Version, version)) != null;
        }

        /// <summary>
        /// Parses the manifest at the specified location and adds the result to
        /// the list of database versions.
        /// </summary>
        /// <param name="manifestInfo">The manifest to parse.</param>
        private void ParseManifest(FileInfo manifestInfo)
        {
            using (Stream fileStream = manifestInfo.Open(FileMode.Open))
            {
                IDatabaseVersion version = this.manifestReader.Read(
                    fileStream, manifestInfo.FullName, this);

                this.versions.Add(version);
            }
        }


        public string GetScriptPath(string manifestPath, string scriptFileName)
        {
            FileInfo manifestFile = new FileInfo(manifestPath);
            string filePath = Path.Combine(manifestFile.Directory.Name, scriptFileName);
            return filePath;
        }
    }
}
