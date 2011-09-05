namespace dbversion.Property
{
    using System.Collections.Generic;

    /// <summary>
    /// An object that contains default property values.
    /// </summary>
    public interface IHaveDefaultProperties
    {
        /// <summary>
        /// Gets the default properties.
        /// </summary>
        /// <value>
        /// The default properties.
        /// </value>
        IEnumerable<Property> DefaultProperties { get; }
    }
}
