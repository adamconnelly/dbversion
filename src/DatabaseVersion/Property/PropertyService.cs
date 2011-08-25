using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace dbversion.Property
{
    [Export(typeof(IPropertyService))]
    public class PropertyService : IPropertyService
    {
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

        public string this[string propertyName]
        {
            get
            {
                string propertyValue;

                if (this.properties.TryGetValue(propertyName, out propertyValue))
                {
                    return propertyValue;
                }

                return null;
            }

            set
            {
                this.properties[propertyName] = value;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> StartingWith(string prefix)
        {
            return this.properties.Where(p => p.Key.StartsWith(prefix));
        }
    }
}

