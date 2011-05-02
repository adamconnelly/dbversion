using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DatabaseVersion
{
    public interface IDatabaseTaskFactory
    {
        bool CanHandle(XmlReader reader);
        IDatabaseTask Create(XmlReader reader);
    }
}
