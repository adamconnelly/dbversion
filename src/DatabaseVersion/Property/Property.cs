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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="dbversion.Property.Property"/>.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="dbversion.Property.Property"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="dbversion.Property.Property"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Property))
            {
                return false;
            }

            Property other = (Property)obj;
            return this.Key == other.Key && this.Value == other.Value;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="dbversion.Property.Property"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Key ?? string.Empty).GetHashCode() ^ (this.Value ?? string.Empty).GetHashCode();
            }
        }

    }
}

