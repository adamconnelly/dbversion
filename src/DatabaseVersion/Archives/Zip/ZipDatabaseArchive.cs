using DatabaseVersion.Manifests;
using Ionic.Zip;
using System.IO;
using System.Linq;
using System.ComponentModel.Composition;
using System.Collections.Generic;
namespace DatabaseVersion.Archives.Zip
{
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
                foreach (ZipEntry entry in zipfile.SelectEntries("database.xml"))
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

        public System.Collections.Generic.IEnumerable<IDatabaseVersion> Versions
        {
            get { return this.versions; }
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
