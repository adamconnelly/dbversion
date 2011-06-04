using System;
namespace DatabaseVersion.Version.NumericVersion
{
    public class NumericVersion
    {
        public NumericVersion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericVersion"/> class.
        /// </summary>
        /// <param name="version">The version number.</param>
        public NumericVersion(int version)
        {
            this.Version = version;
        }

        public virtual int Version { get; set; }
        public virtual DateTime UpdatedOn { get; set; }

        public override bool Equals(object obj)
        {
            NumericVersion version = obj as NumericVersion;
            if (object.ReferenceEquals(version, null))
            {
                return false;
            }

            return object.Equals(this.Version, version.Version);
        }

        public override int GetHashCode()
        {
            return this.Version;
        }
    }
}
