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
        /// <summary>
        /// The name of the properties file.
        /// </summary>
        public const string PropertyFileName = "properties.xml";

        /// <summary>
        /// The properties.
        /// </summary>
        private readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IEnumerable<Property> Properties
        {
            get
            {
                return this.properties.Values;
            }
        }

        /// <summary>
        /// Gets or sets the objects that contain default properties for the service.
        /// </summary>
        /// <value>
        /// The property defaulters.
        /// </value>
        [ImportMany(typeof(IHaveDefaultProperties))]
        public IEnumerable<IHaveDefaultProperties> PropertyDefaulters { get; set; }

        /// <summary>
        /// Gets the <see cref="dbversion.Property.Property"/> with the specified propertyName.
        /// </summary>
        /// <param name='propertyName'>
        /// The name of the property to get.
        /// </param>
        public Property this[string propertyName]
        {
            get
            {
                return this.Get(propertyName);
            }
        }

        public void Add(Property property)
        {
            Validate.NotNull(() => property);

            this.properties[property.Key] = property;
        }

        public void Merge(IEnumerable<Property> properties)
        {
            properties.ForEach(p => this.Add(p));
        }

        /// <summary>
        /// Sets the default properties.
        /// </summary>
        public void SetDefaultProperties()
        {
            this.PropertyDefaulters.ForEach(d => this.Merge(d.DefaultProperties));
        }

        public IEnumerable<Property> StartingWith(string prefix)
        {
            return this.properties.Where(p => p.Key.StartsWith(prefix)).Select(p => p.Value);
        }

        /// <summary>
        /// Gets the specified property as an int.
        /// </summary>
        /// <param name='key'>
        /// The property key.
        /// </param>
        /// <returns>
        /// The int value or null if the property is not defined, or is not an int.
        /// </returns>
        public int? GetInt(string key)
        {
            Validate.NotNull(() => key);

            int value;
            var property = this.Get(key);

            if (property != null && int.TryParse(property.Value, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Get the property with the specified name.
        /// </summary>
        /// <param name='propertyName'>
        /// The name of the property.
        /// </param>
        private Property Get(string propertyName)
        {
            Property propertyValue;
            if (this.properties.TryGetValue(propertyName, out propertyValue))
            {
                return propertyValue;
            }

            return null;
        }
    }
}

