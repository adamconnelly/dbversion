namespace dbversion.Property
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to application properties.
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Gets or sets the property with the specified propertyName.
        /// </summary>
        /// <param name='propertyName'>
        /// The name of the property to set.
        /// </param>
        Property this[string propertyName]
        {
            get;
        }

        /// <summary>
        /// Add the specified property to the service.
        /// </summary>
        /// <param name='property'>
        /// The property to add.
        /// </param>
        void Add(Property property);

        /// <summary>
        /// Merges the specified properties into the service, overwriting any existing
        /// property values.
        /// </summary>
        /// <param name='properties'>
        /// The properties to merge.
        /// </param>
        void Merge(IEnumerable<Property> properties);

        /// <summary>
        /// Sets the default properties.
        /// </summary>
        void SetDefaultProperties();

        /// <summary>
        /// Gets all the properties starting with the specified prefix.
        /// </summary>
        /// <returns>
        /// The properties starting with the specified prefix.
        /// </returns>
        /// <param name='prefix'>
        /// The prefix to look for.
        /// </param>
        IEnumerable<Property> StartingWith(string prefix);
    }
}

