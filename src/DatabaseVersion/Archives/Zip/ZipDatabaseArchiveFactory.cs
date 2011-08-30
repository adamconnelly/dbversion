using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using dbversion.Manifests;
using System.ComponentModel.Composition;

namespace dbversion.Archives.Zip
{
    [Export(typeof(IDatabaseArchiveFactory))]
    public class ZipDatabaseArchiveFactory : IDatabaseArchiveFactory
    {
        [Import]
        public IManifestReader ManifestReader { get; set; }

        public bool CanCreate(string path)
        {
            return ZipFile.IsZipFile(path);
        }

        public IDatabaseArchive Create(string path)
        {
            Validate.NotEmpty(() => path);

            return new ZipDatabaseArchive(path, this.ManifestReader);
        }
    }
}
