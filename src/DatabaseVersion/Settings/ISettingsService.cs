namespace dbversion.Settings
{
    using System.IO;

    /// <summary>
    /// Provides access to application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the directory containing the settings.
        /// </summary>
        /// <value>
        /// The settings directory.
        /// </value>
        string SettingsDirectory
        {
            get;
        }

        /// <summary>
        /// Writes the specified stream to the specified file.
        /// </summary>
        /// <param name='stream'>
        /// The stream to write.
        /// </param>
        /// <param name='fileName'>
        /// The file to write the stream to.
        /// </param>
        void Write(Stream stream, string fileName);

        /// <summary>
        /// Read the specified fileName.
        /// </summary>
        /// <param name='fileName'>
        /// The filename to read.
        /// </param>
        Stream Read(string fileName);

        /// <summary>
        /// Serialize the specified object to the specified file.
        /// </summary>
        /// <param name='object'>
        /// The object to serialize.
        /// </param>
        /// <param name='fileName'>
        /// The file to serialize it to.
        /// </param>
        void Serialize(object @object, string fileName);

        /// <summary>
        /// Deserializes an object from the specified settings file.
        /// </summary>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        /// <param name='fileName'>
        /// The name of the file containing the serialized object.
        /// </param>
        /// <typeparam name='T'>
        /// The type of object to deserialize.
        /// </typeparam>
        T DeSerialize<T>(string fileName);
    }
}

