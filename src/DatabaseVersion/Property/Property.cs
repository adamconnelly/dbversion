namespace dbversion.Property
{
    using System.Xml.Serialization;
    
    public class Property
    {
        [XmlAttribute("key")]
        public string Key
        {
            get;
            set;
        }

        [XmlAttribute("value")]
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="dbversion.Property.Property"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="dbversion.Property.Property"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[Property: Key={0}, Value={1}]", Key, Value);
        }
    }
}

