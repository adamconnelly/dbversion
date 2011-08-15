using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Data;
using DatabaseVersion.Tasks;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Criterion;
using DatabaseVersion.Property;
using NHibernate.Cfg;

namespace DatabaseVersion.Version.ClassicVersion
{
    [Export(typeof(IVersionProvider))]
    public class ClasicVersionProvider : IVersionProvider
    {
        [Import]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        public VersionBase CreateVersion(string versionString)
        {
            return new ClassicVersion(versionString);
        }

        public bool VersionTableExists(ISession session)
        {
            using (IDbCommand command = session.Connection.CreateCommand())
            {
                command.CommandText = "select count(1) from information_schema.tables where table_name = 'Version'";
                var count = Convert.ToInt64(command.ExecuteScalar());

                return count == 1;
            }
        }

        public VersionBase GetCurrentVersion(ISession session)
        {
            return session.QueryOver<ClassicVersion>()
                    .OrderBy(v => v.Version).Desc()
                    .List()
                    .FirstOrDefault();
        }

        public void CreateVersionTable(ISession session)
        {

//            Fluently.Configure()
//                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connection.ConnectionString))
////                .Mappings(v => v.FluentMappings.Add<ClassicVersionMap>())
////                .Mappings(v => v.FluentMappings.Add<ClassicVersionTaskMap>())
//                    .Mappings(v => {
//                v.FluentMappings.Add<ClassicVersionMap>();
//                v.FluentMappings.Add<ClassicVersionTaskMap>();
//            })
//                .ExposeConfiguration(c => new SchemaExport(c).Create(false, true))
//                .BuildSessionFactory();
            Fluently.Configure(this.GetConfiguration())
                .Mappings(v => v.FluentMappings.Add<ClassicVersionMap>())
                    .Mappings(v => v.FluentMappings.Add<ClassicVersionTaskMap>())
                    .ExposeConfiguration(c => new SchemaExport(c).Create(false, true))
                    .BuildSessionFactory();
        }

        public void InsertVersion(VersionBase version, ISession session)
        {
            ClassicVersion classicVersion = version as ClassicVersion;
            if (!classicVersion.CreatedOn.HasValue)
            {
                classicVersion.CreatedOn = DateTime.UtcNow;
            }
            classicVersion.UpdatedOn = DateTime.UtcNow;
            session.SaveOrUpdate(version);
        }

        public IComparer<object> GetComparer()
        {
            return new ClassicVersionComparer();
        }

        private ISessionFactory CreateSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.Add<ClassicVersionMap>())
                .Mappings(m => m.FluentMappings.Add<ClassicVersionTaskMap>())
                .BuildSessionFactory();
        }

        public bool HasExecutedScript(VersionBase currentVersion, VersionBase targetVersion, IDatabaseTask task)
        {
            if (currentVersion != null)
            {
                ClassicVersion currentClassicVersion = currentVersion as ClassicVersion;
                ClassicVersion targetClassicVersion = targetVersion as ClassicVersion;
                if (currentClassicVersion.SystemVersion.Equals(targetClassicVersion.SystemVersion))
                {
                    return currentClassicVersion.HasExecutedTask(task);
                }
            }

            return false;
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

        private class ClassicVersionComparer : Comparer<object>
        {
            public override int Compare(object x, object y)
            {
                ClassicVersion left = x as ClassicVersion;
                ClassicVersion right = y as ClassicVersion;

                return left.Version.CompareTo(right.Version);
            }
        }
    }
}
