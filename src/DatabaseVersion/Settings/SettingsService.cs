namespace dbversion.Settings
{
    using System;
    using System.IO;

    using dbversion.Utils;

    public class SettingsService
    {
        public const string DirectoryName = "dbversion";

        public string SettingsDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DirectoryName);
            }
        }

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

        public void Serialize(object @object, string fileName)
        {
            Validate.NotNull(() => @object);
            Validate.NotEmpty(() => fileName);

            this.Write(XmlSerializer.Serialize(@object), fileName);
        }
    }
}

