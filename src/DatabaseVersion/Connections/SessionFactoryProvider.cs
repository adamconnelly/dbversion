using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using DatabaseVersion.Property;

namespace DatabaseVersion.Connections
{
    [Export(typeof(ISessionFactoryProvider))]
    public class SessionFactoryProvider : ISessionFactoryProvider
    {
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

        public ISessionFactory CreateSessionFactory()
        {
            FluentConfiguration configuration = Fluently.Configure(this.GetConfiguration());

            AddMappings(configuration);

            return configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private Configuration GetConfiguration()
        {
            Dictionary<string, string > properties = new Dictionary<string, string>();
//            properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
//            properties["connection.driver_class"] = "NHibernate.Driver.MySqlDataDriver";
//            properties["connection.connection_string"] = "Server=localhost;Database=person;Uid=adam;";
//            properties["dialect"] = "NHibernate.Dialect.MySQLDialect";
            properties["connection.provider"] = this.PropertyService["connection.provider"];
            properties["connection.driver_class"] = this.PropertyService["connection.driver_class"];
            properties["connection.connection_string"] = this.PropertyService["connection.connection_string"];
            properties["dialect"] = this.PropertyService["dialect"];

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

