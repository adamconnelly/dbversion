namespace dbversion.Property
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using dbversion.Utils;

    [Export(typeof(IPropertyService))]
    public class PropertyService : IPropertyService
    {
        private readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();

        public Property this[string propertyName]
        {
            get
            {
                Property propertyValue;

                if (this.properties.TryGetValue(propertyName, out propertyValue))
                {
                    return propertyValue;
                }

                return null;
            }
        }

        public void Add(Property property)
        {
            Validate.NotNull(() => property);

            this.properties[property.Key] = property;
        }

        public void Merge(IEnumerable<Property> properties)
        {
            properties.ForEach(p => this.properties[p.Key] = p);
        }

        public IEnumerable<Property> StartingWith(string prefix)
        {
            return this.properties.Where(p => p.Key.StartsWith(prefix)).Select(p => p.Value);
        }
    }
}

