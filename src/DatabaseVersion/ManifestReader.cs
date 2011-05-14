using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Xml.Linq;
using DatabaseVersion.Archives;

namespace DatabaseVersion
{
    public class ManifestReader
    {
        [ImportMany]
        public IEnumerable<IDatabaseTaskFactory> Factories { get; set; }

        public IDatabaseVersion Read(Stream stream, string manifestPath, IDatabaseArchive archive)
        {
            Validate.NotNull(() => stream);
            Validate.NotNull(() => manifestPath);
            Validate.NotNull(() => archive);

            XmlReader reader = XmlReader.Create(stream);
            reader.MoveToContent();
            XElement element = XElement.ReadFrom(reader) as XElement;

            string version = element.Attributes().First(a => a.Name == "version").Value;

            DatabaseVersion databaseVersion = new DatabaseVersion(version, manifestPath, archive);
            databaseVersion.Tasks = CreateTasks(element, databaseVersion);

            return databaseVersion;
        }

        private IEnumerable<IDatabaseTask> CreateTasks(XElement element, IDatabaseVersion version)
        {
            return element.Elements()
                .Select(e => this.CreateTask(e, version))
                .OfType<IDatabaseTask>(); // Use to remove nulls
        }

        private IDatabaseTask CreateTask(XElement element, IDatabaseVersion version)
        {
            if (this.Factories != null)
            {
                IDatabaseTaskFactory factory = this.Factories.FirstOrDefault(f => f.CanCreate(element));
                if (factory != null)
                {
                    return factory.Create(element, element.ElementsBeforeSelf().Count(), version);
                }
            }

            return null;
        }
    }
}
