namespace dbversion.Archives.Zip
{
    using System;
    using System.IO;
    using System.Linq;
    using System.ComponentModel.Composition;
    using System.Collections.Generic;

    using dbversion.Manifests;
    using dbversion.Property;
    using dbversion.Version;

    using Ionic.Zip;

    [Export(typeof(IDatabaseArchive))]
    public class ZipDatabaseArchive : IDatabaseArchive
    {
        #region Fields
        private readonly IManifestReader manifestReader;
        private readonly List<IDatabaseVersion> versions = new List<IDatabaseVersion>();

        #endregion

        public ZipDatabaseArchive(string path, IManifestReader manifestReader)
        {
            Validate.NotEmpty(() => path);
            Validate.NotNull(() => manifestReader);

            this.ArchivePath = path;
            this.manifestReader = manifestReader;

            using (ZipFile zipfile = new ZipFile(path))
            {
                foreach (ZipEntry entry in zipfile.Where(e => e.FileName.EndsWith("database.xml")))
                {
                    this.ParseManifest(entry);
                }
            }
        }

        public string ArchivePath
        {
            get;
            private set;
        }

        public IEnumerable<IDatabaseVersion> Versions
        {
            get { return this.versions; }
        }

        public IEnumerable<Property> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public System.IO.Stream GetFile(string path)
        {
            using (ZipFile zipFile = new ZipFile(this.ArchivePath))
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

        public bool ContainsVersion(object version)
        {
            return this.Versions.FirstOrDefault(v => object.Equals(v.Version, version)) != null;
        }

        private void ParseManifest(ZipEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            entry.Extract(stream);
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            this.versions.Add(this.manifestReader.Read(stream, entry.FileName, this));
        }

        public string GetScriptPath(string manifestPath, string scriptFileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(manifestPath), scriptFileName);
            return filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}
