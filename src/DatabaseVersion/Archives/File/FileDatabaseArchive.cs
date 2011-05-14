using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseVersion.Archives.File
{
    public class FileDatabaseArchive : IDatabaseArchive
    {
        public FileDatabaseArchive(string archivePath)
        {
            this.ArchivePath = archivePath;
        }

        public IEnumerable<IDatabaseVersion> Versions
        {
            get { throw new NotImplementedException(); }
        }

        public System.IO.Stream GetFile(string path)
        {
            throw new NotImplementedException();
        }

        public string ArchivePath
        {
            get;
            private set;
        }
    }
}
