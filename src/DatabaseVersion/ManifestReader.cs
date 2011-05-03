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

            return new DatabaseVersion(version, manifestPath, CreateTasks(element));
        }

        private IEnumerable<IDatabaseTask> CreateTasks(XElement element)
        {
            return element.Elements()
                .Select(e => this.CreateTask(e))
                .OfType<IDatabaseTask>(); // Use to remove nulls
        }

        private IDatabaseTask CreateTask(XElement element)
        {
            if (this.Factories != null)
            {
                IDatabaseTaskFactory factory = this.Factories.FirstOrDefault(f => f.CanCreate(element));
                if (factory != null)
                {
                    return factory.Create(element, element.ElementsBeforeSelf().Count());
                }
            }

            return null;
        }
    }
}
