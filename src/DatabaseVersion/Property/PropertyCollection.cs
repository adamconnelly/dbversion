namespace dbversion.Property
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Contains a list of properties.
    /// </summary>
    [XmlRoot("properties")]
    public class PropertyCollection
    {
        /// <summary>
        /// The backing field for <see cref="Properties"/>.
        /// </summary>
        private readonly List<Property> properties = new List<Property>();

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [XmlElement("property")]
        public List<Property> Properties
        {
            get
            {
                return this.properties;
            }
        }
    }
}

