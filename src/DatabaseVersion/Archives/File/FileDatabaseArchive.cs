namespace dbversion.Archives.File
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.ComponentModel.Composition;
    
    using dbversion.Manifests;
    using dbversion.Version;

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
        private List<IDatabaseVersion> versions = new List<IDatabaseVersion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDatabaseArchive"/> class.
        /// </summary>
        /// <param name="archivePath">The root directory of the archive.</param>
        public FileDatabaseArchive(string archivePath, IManifestReader manifestReader)
        {
            this.ArchivePath = archivePath;
            this.manifestReader = manifestReader;
            this.ParseManifests();
        }

        /// <summary>
        /// Gets the list of database versions from the archive.
        /// </summary>
        public IEnumerable<IDatabaseVersion> Versions
        {
            get
            {
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
        /// Parses the manifests.
        /// </summary>
        private void ParseManifests()
        {
            var directoryInfo = new DirectoryInfo(this.ArchivePath);
            var manifests = directoryInfo.GetFiles("database.xml", SearchOption.AllDirectories);
        	
            foreach (var manifest in manifests)
            {
                this.versions.Add(ParseManifest(manifest));
            }
        }

        /// <summary>
        /// Parses the manifest at the specified location.
        /// </summary>
        /// <param name="manifestInfo">The manifest to parse.</param>
        /// <returns>The parsed manifest.</returns>
        private IDatabaseVersion ParseManifest(FileInfo manifestInfo)
        {
            using (Stream fileStream = manifestInfo.Open(FileMode.Open))
            {
                return this.manifestReader.Read(
                    fileStream, manifestInfo.FullName, this);
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
