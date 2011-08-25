using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace dbversion.Tests.Utils
{
    public static class AssemblyUtil
    {
        public static void CopyContentsToDirectory(string root, string directory)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.StartsWith(root))
                {
                    string fileName = name.Substring(root.Length);
                    fileName = fileName.Replace('.', Path.DirectorySeparatorChar);
                    int index = fileName.LastIndexOf(Path.DirectorySeparatorChar);

                    fileName = fileName.Substring(0, index) + "." + fileName.Substring(index + 1);
                    string fileDirectory = fileName.Substring(0, fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    DirectoryInfo directoryInfo = new DirectoryInfo(directory + fileDirectory);
                    
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    FileInfo fileInfo = new FileInfo(directory + fileName);

                    using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(name)))
                    using (StreamWriter writer = new StreamWriter(fileInfo.Create()))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
