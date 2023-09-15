namespace dbversion.Archives.Zip
{
    using System;
    using System.IO;
    using System.Linq;
    using System.ComponentModel.Composition;
    using System.Collections.Generic;

    using dbversion.Manifests;
    using dbversion.Property;
    using dbversion.Utils;
    using dbversion.Version;
    using Ionic.Zip;

    /// <summary>
    /// A database archive that stores its files in a zip file.
    /// </summary>
    [Export(typeof(IDatabaseArchive))]
    public class ZipDatabaseArchive : IDatabaseArchive
    {
        #region Fields

        /// <summary>
        /// The manifest reader.
        /// </summary>
        private readonly IManifestReader manifestReader;

        /// <summary>
        /// The versions contained in the archive.
        /// </summary>
        private readonly List<IDatabaseVersion> versions = new List<IDatabaseVersion>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="dbversion.Archives.Zip.ZipDatabaseArchive"/> class.
        /// </summary>
        /// <param name='path'>
        /// The path to the archive.
        /// </param>
        /// <param name='manifestReader'>
        /// The manifest reader.
        /// </param>
        public ZipDatabaseArchive(string path, IManifestReader manifestReader)
        {
            Validate.NotEmpty(() => path);
            Validate.NotNull(() => manifestReader);

            this.ArchivePath = path;
            this.manifestReader = manifestReader;

            using (ZipFile zipFile = new ZipFile(path))
            {
                this.ParseManifests(zipFile);
                this.LoadProperties(zipFile);
            }
        }

        /// <summary>
        /// Gets the archive path.
        /// </summary>
        /// <value>
        /// The archive path.
        /// </value>
        public string ArchivePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the versions contained in the archive.
        /// </summary>
        /// <value>
        /// The versions.
        /// </value>
        public IEnumerable<IDatabaseVersion> Versions
        {
            get { return this.versions; }
        }

        /// <summary>
        /// Gets the properties set in the archive.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IEnumerable<Property> Properties
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the specified file from the archive.
        /// </summary>
        /// <param name='path'>
        /// The path to the file.
        /// </param>
        /// <returns>
        /// The file or null if the file does not exist.
        /// </returns>
        public System.IO.Stream GetFile(string path)
        {
            using (var zipFile = new ZipFile(this.ArchivePath))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    if (entry.FileName == path)
                    {
                        MemoryStream stream = new MemoryStream();
                        entry.Extract(stream);
                        
                        return stream;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns whether the archive contains the specified version.
        /// </summary>
        /// <param name='version'>
        /// The version to look for.
        /// </param>
        /// <returns>
        /// true if the archive contains the version, false otherwise.
        /// </returns>
        public bool ContainsVersion(object version)
        {
            return this.Versions.FirstOrDefault(v => object.Equals(v.Version, version)) != null;
        }

        /// <summary>
        /// Gets the script path.
        /// </summary>
        /// <param name='manifestPath'>
        /// The manifest path.
        /// </param>
        /// <param name='scriptFileName'>
        /// The script file name.
        /// </param>
        /// <returns>
        /// The script path.
        /// </returns>
        public string GetScriptPath(string manifestPath, string scriptFileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(manifestPath), scriptFileName);
            return filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Parses the manifests contained in the zip file.
        /// </summary>
        /// <param name='zipFile'>
        /// The zip file.
        /// </param>
        private void ParseManifests(ZipFile zipFile)
        {
            foreach (ZipEntry entry in zipFile.Where(e => e.FileName.EndsWith("database.xml")))
            {
                this.ParseManifest(entry);
            }
        }

        /// <summary>
        /// Parses the manifest.
        /// </summary>
        /// <param name='entry'>
        /// The zip entry containing the manifest.
        /// </param>
        private void ParseManifest(ZipEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            entry.Extract(stream);
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            this.versions.Add(this.manifestReader.Read(stream, entry.FileName, this));
        }

        /// <summary>
        /// Loads the properties.
        /// </summary>
        /// <param name='zipFile'>
        /// The zip file to load the properties from.
        /// </param>
        private void LoadProperties(ZipFile zipFile)
        {
            if (zipFile.ContainsEntry(PropertyService.PropertyFileName))
            {
                var propertiesFile = zipFile.Single(p => p.FileName == PropertyService.PropertyFileName);
                this.Properties = XmlSerializer.DeSerialize<PropertyCollection>(propertiesFile.OpenReader()).Properties;
            }
            else
            {
                this.Properties = Enumerable.Empty<Property>();
            }
        }
    }
}
