namespace dbversion.Utils
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Contains utility methods for serializing and de-serializing objects.
    /// </summary>
    public static class XmlSerializer
    {
        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name='object'>
        /// The object to Serialize.
        /// </param>
        public static Stream Serialize(object @object)
        {
            Validate.NotNull(() => @object);

            var serializer = new System.Xml.Serialization.XmlSerializer(@object.GetType());

            var stream = new MemoryStream();
            serializer.Serialize(stream, @object);

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// De-Serializes the specified stream.
        /// </summary>
        /// <returns>
        /// The De-Serialized object.
        /// </returns>
        /// <param name='inputStream'>
        /// The stream containing the object to De-Serialize.
        /// </param>
        /// <typeparam name='T'>
        /// The type of the object to De-Serialize.
        /// </typeparam>
        public static T DeSerialize<T>(Stream inputStream)
        {
            Validate.NotNull(() => inputStream);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(inputStream);
        }
    }
}

