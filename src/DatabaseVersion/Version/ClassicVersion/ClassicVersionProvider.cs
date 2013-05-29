namespace dbversion.Version.ClassicVersion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.Composition;
    using dbversion.Tasks;
    using NHibernate;
    using FluentNHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NHibernate.Cfg;
    using dbversion.Session;
    using dbversion.Version;

    [Export(typeof(IVersionProvider))]
    public class ClassicVersionProvider : IVersionProvider
    {
        [Import]
        public ISessionFactoryProvider SessionFactoryProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a version object from its string representation.
        /// </summary>
        /// <param name="versionString">The string representation.</param>
        /// <returns>The version object.</returns>
        public VersionBase CreateVersion(string versionString)
        {
            return new ClassicVersion(versionString);
        }

        /// <summary>
        /// Checks whether the version table exists in the database.
        /// </summary>
        /// <param name="session">The database connection.</param>
        /// <returns>true if the table exists, false otherwise.</returns>
        public bool VersionTableExists(ISession session)
        {
            // TODO: Figure out how to get NHibernate to check this since this way we can't be sure the table name is correct
            var query = session.CreateSQLQuery("select count(1) from information_schema.tables where table_name = 'version'");
            return Convert.ToInt64(query.UniqueResult()) == 1;
        }

        /// <summary>
        /// Gets the latest installed version from the database.
        /// </summary>
        /// <param name="session">The database connection.</param>
        /// <returns>
        /// The latest version installed in the database or null if
        /// no versions have been installed.
        /// </returns>
        public VersionBase GetCurrentVersion(ISession session)
        {
            return session.QueryOver<ClassicVersion>()
                .List()
                .OrderByDescending(v => v, this.GetComparer())
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the specified version from the database.
        /// </summary>
        /// <param name="session">The database connection.</param>
        /// <param name="version">The version to get.</param>
        /// <returns>The version or null if the version has not been installed in the database.</returns>
        public VersionBase GetVersion(ISession session, VersionBase version)
        {
            var classicVersion = version as ClassicVersion;

            return
                session.QueryOver<ClassicVersion>().Where(v => v.Version == classicVersion.Version).SingleOrDefault();
        }

        /// <summary>
        /// Creates the version table.
        /// </summary>
        /// <param name="session">The database connection.</param>
        public void CreateVersionTable(ISession session)
        {
            Configuration config = null;
            Fluently.Configure(this.SessionFactoryProvider.GetConfiguration())
                .Mappings(v => v.FluentMappings.Add<ClassicVersionMap>())
                    .Mappings(v => v.FluentMappings.Add<ClassicVersionTaskMap>())
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

        /// <summary>
        /// Inserts the specified version into the version table.
        /// </summary>
        /// <param name="version">The version to insert.</param>
        /// <param name="session">The database connection.</param>
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

        /// <summary>
        /// Returns an object that can be used to compare two version objects
        /// created by this provider.
        /// </summary>
        /// <returns>The comparer.</returns>
        public IComparer<object> GetComparer()
        {
            return new ClassicVersionComparer();
        }

        /// <summary>
        /// Returns whether a script belongs to the current version and if so whether it has already been executed
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="targetVersion"></param>
        /// <param name="task"></param>
        /// <returns></returns>
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
        /// Gets all the versions that have been installed.
        /// </summary>
        public IEnumerable<VersionBase> GetAllVersions(ISession session)
        {
            return session.QueryOver<ClassicVersion>().List();
        }

        public class ClassicVersionComparer : Comparer<object>
        {
            public override int Compare(object x, object y)
            {
                ClassicVersion left = x as ClassicVersion;
                ClassicVersion right = y as ClassicVersion;

                return left.SystemVersion.CompareTo(right.SystemVersion);
            }
        }
    }
}