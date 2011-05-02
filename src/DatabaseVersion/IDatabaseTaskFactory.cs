using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DatabaseVersion
{
    public interface IDatabaseTaskFactory
    {
        bool CanHandle(XElement element);
        IDatabaseTask Create(XElement element);
    }
}
