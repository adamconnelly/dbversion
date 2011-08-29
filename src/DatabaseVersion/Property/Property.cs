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
    }
}

