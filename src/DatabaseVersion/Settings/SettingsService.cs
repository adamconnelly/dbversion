namespace dbversion.Settings
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;

    using dbversion.Utils;

    /// <summary>
    /// Provides access to application settings.
    /// </summary>
    [Export(typeof(ISettingsService))]
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// The name of the directory containing the settings.
        /// </summary>
        public const string DirectoryName = "dbversion";

        /// <summary>
        /// Gets the full path to the directory containing the settings files.
        /// </summary>
        /// <value>
        /// The settings directory.
        /// </value>
        public string SettingsDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DirectoryName);
            }
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
        public void Write(Stream stream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(this.SettingsDirectory);
            if (!info.Exists)
            {
                info.Create();
            }

            Validate.NotNull(() => stream);
            Validate.NotEmpty(() => fileName);

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            using (var fileStream = new FileStream(Path.Combine(this.SettingsDirectory, fileName), FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Read the specified fileName.
        /// </summary>
        /// <param name='fileName'>
        /// The filename to read.
        /// </param>
        public Stream Read(string fileName)
        {
            Validate.NotEmpty(() => fileName);

            var fileInfo = new FileInfo(Path.Combine(this.SettingsDirectory, fileName));

            if (!fileInfo.Exists)
            {
                return null;
            }

            return fileInfo.OpenRead();
        }

        /// <summary>
        /// Serialize the specified object to the specified file.
        /// </summary>
        /// <param name='object'>
        /// The object to serialize.
        /// </param>
        /// <param name='fileName'>
        /// The file to serialize it to.
        /// </param>
        public void Serialize(object @object, string fileName)
        {
            Validate.NotNull(() => @object);
            Validate.NotEmpty(() => fileName);

            this.Write(XmlSerializer.Serialize(@object), fileName);
        }

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
        public T DeSerialize<T>(string fileName)
        {
            Validate.NotEmpty(() => fileName);

            Stream stream = this.Read(fileName);

            if (stream != null)
            {
                try
                {
                    return XmlSerializer.DeSerialize<T>(stream);
                }
                catch (Exception)
                {
                    // TODO: Add logging?
                    return default(T);
                }
                finally
                {
                    stream.Dispose();
                }
            }

            return default(T);
        }
    }
}

