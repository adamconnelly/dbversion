namespace dbversion.Tasks.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.ComponentModel.Composition;

    using dbversion.Property;
    using dbversion.Version;

    [Export(typeof(IDatabaseTaskFactory))]
    public class ScriptTaskFactory : IDatabaseTaskFactory
    {
        [Import]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        [Import]
        public IMessageService MessageService 
        { 
            get; 
            set; 
        }

        public bool CanCreate(XElement element)
        {
            return element != null && element.Name == "script";
        }

        public IDatabaseTask Create(XElement element, int executionOrder, IDatabaseVersion version)
        {
            return new ScriptTask(element.Attribute(XName.Get("file")).Value, executionOrder, version, this.MessageService, this.PropertyService);
        }
    }
}
