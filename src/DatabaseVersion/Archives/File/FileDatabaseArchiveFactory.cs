using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using DatabaseVersion.Manifests;

namespace DatabaseVersion.Archives.File
{
    [Export(typeof(IDatabaseArchiveFactory))]
    public class FileDatabaseArchiveFactory : IDatabaseArchiveFactory
    {
        [Import]
        public IManifestReader ManifestReader
        {
            get;
            set;
        }

        public bool CanCreate(string path)
        {
            return path != null && new DirectoryInfo(path).Exists;
        }

        public IDatabaseArchive Create(string path)
        {
            Validate.NotEmpty(() => path);

            return new FileDatabaseArchive(path, this.ManifestReader);
        }
    }
}
