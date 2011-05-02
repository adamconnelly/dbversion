using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel.Composition;

namespace DatabaseVersion
{
    public class ManifestReader
    {
        [ImportMany]
        public IEnumerable<IDatabaseTaskFactory> Factories { get; set; }

        public IDatabaseVersion Read(Stream stream, string manifestPath)
        {
            Validate.NotNull(stream, "stream");
            Validate.NotNull(manifestPath, "manifestPath");

            XmlReader reader = XmlReader.Create(stream);
            reader.Read();
            
            string version = reader.GetAttribute("version");

            List<IDatabaseTask> tasks = new List<IDatabaseTask>();

            reader = reader.ReadSubtree();
            while (reader.Read() && reader.NodeType == XmlNodeType.Element)
            {
                IDatabaseTask task = this.CreateTask(reader);
                if (task != null)
                {
                    tasks.Add(task);
                }
            }

            return new DatabaseVersion(version, manifestPath, tasks);
        }

        private IDatabaseTask CreateTask(XmlReader reader)
        {
            if (this.Factories != null)
            {
                foreach (IDatabaseTaskFactory factory in this.Factories)
                {
                    if (factory.CanHandle(reader))
                    {
                        return factory.Create(reader);
                    }
                }
            }

            return null;
        }
    }
}
