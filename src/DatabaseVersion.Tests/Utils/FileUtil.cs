using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace dbversion.Tests.Utils
{
    public static class FileUtil
    {
        public static DirectoryInfo CreateTempDirectory()
        {
            DirectoryInfo info = null;
            string tempDirectory = Path.GetTempPath();

            do
            {
                string directoryName = Path.GetRandomFileName();

                info = new DirectoryInfo(Path.Combine(tempDirectory, directoryName));
            }
            while (info.Exists);

            info.Create();

            return info;
        }
    }
}
