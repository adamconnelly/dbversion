using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Xml.Linq;

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
            reader.MoveToContent();
            XElement element = XElement.ReadFrom(reader) as XElement;

            string version = element.Attributes().First(a => a.Name == "version").Value;

            List<IDatabaseTask> tasks = new List<IDatabaseTask>();

            if (element.Elements().Count() > 0)
            {
                foreach (XElement child in element.Elements())
                {
                    IDatabaseTask task = this.CreateTask(child);
                    if (task != null)
                    {
                        tasks.Add(task);
                    }
                }
            }

            return new DatabaseVersion(version, manifestPath, tasks);
        }

        private IDatabaseTask CreateTask(XElement element)
        {
            if (this.Factories != null)
            {
                IDatabaseTaskFactory factory = this.Factories.FirstOrDefault(f => f.CanHandle(element));
                if (factory != null)
                {
                    return factory.Create(element);
                }
            }

            return null;
        }
    }
}
