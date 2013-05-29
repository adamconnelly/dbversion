using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Data;
using dbversion.Tasks;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Criterion;
using NHibernate.Cfg;
using dbversion.Session;

namespace dbversion.Version.NumericVersion
{
    //[Export(typeof(IVersionProvider))]
    public class NumericVersionProvider : IVersionProvider
    {
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider
        {
            get;
            set;
        }

        public VersionBase CreateVersion(string versionString)
        {
            return new NumericVersion(int.Parse(versionString));
        }

        public bool VersionTableExists(ISession session)
        {
            var query = session.CreateSQLQuery("select count(1) from information_schema.tables where table_name = 'version'");
            return Convert.ToInt64(query.UniqueResult()) == 1;
        }

        public VersionBase GetCurrentVersion(ISession session)
        {
            return session.QueryOver<NumericVersion>()
                    .OrderBy(v => v.Version).Desc()
                    .List()
                    .FirstOrDefault();
        }

        public VersionBase GetVersion(ISession session, VersionBase requiredVersion)
        {
            var numericVersion = requiredVersion as NumericVersion;

            return session.QueryOver<NumericVersion>().Where(v => v.Version == numericVersion.Version).SingleOrDefault();
        }

        public void CreateVersionTable(ISession session)
        {
            Configuration config = null;
            Fluently.Configure(this.SessionFactoryProvider.GetConfiguration())
                .Mappings(v => v.FluentMappings.Add<NumericVersionMap>())
                    .Mappings(v => v.FluentMappings.Add<NumericVersionTaskMap>())
                    .ExposeConfiguration(c => config = c)
                    .BuildSessionFactory();

            SchemaExport export = new SchemaExport(config);
            export.Create(schema =>
            {
                var query = session.CreateSQLQuery(schema);
                query.ExecuteUpdate();
            },
            false);
        }

        public void InsertVersion(VersionBase version, ISession session)
        {
            NumericVersion numericVersion = version as NumericVersion;
            if (!numericVersion.CreatedOn.HasValue)
            {
                numericVersion.CreatedOn = DateTime.UtcNow;
            }
            numericVersion.UpdatedOn = DateTime.UtcNow;
            session.SaveOrUpdate(version);
        }

        public IComparer<object> GetComparer()
        {
            return new NumericVersionComparer();
        }

        private ISessionFactory CreateSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.Add<NumericVersionMap>())
                .Mappings(m => m.FluentMappings.Add<NumericVersionTaskMap>())
                .BuildSessionFactory();
        }

        public bool HasExecutedScript(VersionBase currentVersion, VersionBase targetVersion, IDatabaseTask task)
        {
            if (currentVersion != null)
            {
                NumericVersion currentNumericVersion = currentVersion as NumericVersion;
                NumericVersion targetNumericVersion = targetVersion as NumericVersion;
                if (currentNumericVersion.Version.Equals(targetNumericVersion.Version))
                {
                    return currentNumericVersion.HasExecutedTask(task);
                }
            }

            return false;
        }

        public IEnumerable<VersionBase> GetAllVersions(ISession session)
        {
            return session.QueryOver<NumericVersion>().List();
        }
        
        private class NumericVersionComparer : Comparer<object>
        {
            public override int Compare(object x, object y)
            {
                NumericVersion left = x as NumericVersion;
                NumericVersion right = y as NumericVersion;

                return left.Version - right.Version;
            }
        }
    }
}
