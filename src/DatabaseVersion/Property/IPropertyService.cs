using System;
using System.Collections.Generic;

namespace dbversion.Property
{
    public interface IPropertyService
    {
        string this[string propertyName]
        {
            get; set;
        }

        IEnumerable<KeyValuePair<string, string>> StartingWith(string prefix);
    }
}

