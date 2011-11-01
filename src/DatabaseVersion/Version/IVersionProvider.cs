using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using dbversion.Tasks;
using dbversion.Version;
using NHibernate;

namespace dbversion
{
    /// <summary>
    /// Provides methods used to create version objects that can be compared
    /// and methods used to interact with the database.
    /// </summary>
    public interface IVersionProvider
    {
        /// <summary>
        /// Creates a version object from its string representation.
        /// </summary>
        /// <param name="versionString">The string representation.</param>
        /// <returns>The version object.</returns>
        VersionBase CreateVersion(string versionString);

        /// <summary>
        /// Checks whether the version table exists in the database.
        /// </summary>
        /// <param name="session">The database connection.</param>
        /// <returns>true if the table exists, false otherwise.</returns>
        bool VersionTableExists(ISession session);

        /// <summary>
        /// Gets the latest installed version from the database.
        /// </summary>
        /// <param name="session">The database connection.</param>
        /// <returns>
        /// The latest version installed in the database or null if
        /// no versions have been installed.
        /// </returns>
        VersionBase GetCurrentVersion(ISession session);

        /// <summary>
        /// Creates the version table.
        /// </summary>
        /// <param name="session">The database connection.</param>
        void CreateVersionTable(ISession session);

        /// <summary>
        /// Inserts the specified version into the version table.
        /// </summary>
        /// <param name="version">The version to insert.</param>
        /// <param name="session">The database connection.</param>
        void InsertVersion(VersionBase version, ISession connection);

        /// <summary>
        /// Returns an object that can be used to compare two version objects
        /// created by this provider.
        /// </summary>
        /// <returns>The comparer.</returns>
        IComparer<object> GetComparer();

        /// <summary>
        /// Returns whether a script belongs to the current version and if so whether it has already been executed
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="targetVersion"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        bool HasExecutedScript(VersionBase currentVersion, VersionBase targetVersion, IDatabaseTask task);

        /// <summary>
        /// Gets all the versions that have been installed.
        /// </summary>
        IEnumerable<VersionBase> GetAllVersions(ISession session);
    }
}
