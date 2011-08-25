namespace dbversion.Version.ClassicVersion
{
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

    using dbversion.Property;
    using dbversion.Session;
    using dbversion.Version;

    [Export(typeof(IVersionProvider))]
    public class ClasicVersionProvider : IVersionProvider
    {
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider
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
            Fluently.Configure(this.SessionFactoryProvider.GetConfiguration())
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
