namespace DatabaseVersion.Version
{
    using System;

    /// <summary>
    /// Thrown if a version cannot be found in the archive.
    /// </summary>
    public class VersionNotFoundException : Exception
    {
        public VersionNotFoundException(object version)
            : base(string.Format("The specified version, {0}, was not found in the archive.", version))
        {
        }
    }
}
