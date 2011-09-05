namespace dbversion.Session
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using FluentNHibernate.Cfg;
    using NHibernate;
    using NHibernate.Cfg;
    using dbversion.Property;

    [Export(typeof(ISessionFactoryProvider))]
    [Export(typeof(IHaveDefaultProperties))]
    public class SessionFactoryProvider : ISessionFactoryProvider, IHaveDefaultProperties
    {
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
                yield return new Property { Key = "hibernate.connection.provider", Value = "NHibernate.Connection.DriverConnectionProvider" };
                yield return new Property { Key = "hibernate.connection.driver_class", Value = "NHibernate.Driver.SqlClientDriver" };
                yield return new Property { Key = "hibernate.dialect", Value = "NHibernate.Dialect.MsSql2008Dialect" };
            }
        }

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

