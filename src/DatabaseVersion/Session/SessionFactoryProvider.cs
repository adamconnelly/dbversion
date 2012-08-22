namespace dbversion.Session
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using dbversion.Property;

    using NHibernate;
    using NHibernate.Cfg;

    using FluentNHibernate.Cfg;

    [Export(typeof(ISessionFactoryProvider))]
    [Export(typeof(IHaveDefaultProperties))]
    public class SessionFactoryProvider : ISessionFactoryProvider, IHaveDefaultProperties
    {
        /// <summary>
        /// The key of the connection string property.
        /// </summary>
        public const string ConnectionStringProperty = "hibernate.connection.connection_string";

        /// <summary>
        /// The key of the connection provider property.
        /// </summary>
        public const string ProviderProperty = "hibernate.connection.provider";

        /// <summary>
        /// The key of the driver_class property.
        /// </summary>
        public const string DriverClassProperty = "hibernate.connection.driver_class";

        /// <summary>
        /// The key of the dialect property.
        /// </summary>
        public const string DialectProperty = "hibernate.dialect";

        /// <summary>
        /// The prefix for any properties that should be passed to the hibernate configuration.
        /// </summary>
        private const string PropertyPrefix = "hibernate.";

        [Import]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<IHibernateMapping> Mappings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the default properties.
        /// </summary>
        /// <value>
        /// The default properties.
        /// </value>
        public IEnumerable<Property> DefaultProperties
        {
            get
            {
                yield return new Property { Key = ProviderProperty, Value = "NHibernate.Connection.DriverConnectionProvider" };
                yield return new Property { Key = DriverClassProperty, Value = "NHibernate.Driver.SqlClientDriver" };
                yield return new Property { Key = DialectProperty, Value = "NHibernate.Dialect.MsSql2008Dialect" };
            }
        }

        /// <summary>
        /// Creates the session factory.
        /// </summary>
        /// <returns>
        /// The session factory.
        /// </returns>
        public ISessionFactory CreateSessionFactory()
        {
            FluentConfiguration configuration = Fluently.Configure(this.GetConfiguration());

            AddMappings(configuration);

            return configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Gets the configuration with no mappings added.
        /// </summary>
        public Configuration GetConfiguration()
        {
            Dictionary<string, string > properties = new Dictionary<string, string>();

            foreach (var property in this.PropertyService.StartingWith(PropertyPrefix))
            {
                properties[property.Key.Substring(PropertyPrefix.Length)] = property.Value;
            }

            return new Configuration().AddProperties(properties);
        }

        private void AddMappings(FluentConfiguration configuration)
        {
            configuration.Mappings(m => {
                foreach (var mapping in this.Mappings)
                {
                    m.FluentMappings.Add(mapping.GetType());
                }
            });
        }
    }
}

