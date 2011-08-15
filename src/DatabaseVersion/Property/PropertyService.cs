using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DatabaseVersion.Property
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
    }
}

