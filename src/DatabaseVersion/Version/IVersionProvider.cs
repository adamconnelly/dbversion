using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseVersion.Tasks;
using DatabaseVersion.Version;

namespace DatabaseVersion
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
        /// <param name="connection">The database connection.</param>
        /// <returns>true if the table exists, false otherwise.</returns>
        bool VersionTableExists(IDbConnection connection);

        /// <summary>
        /// Gets the latest installed version from the database.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>
        /// The latest version installed in the database or null if
        /// no versions have been installed.
        /// </returns>
        VersionBase GetCurrentVersion(IDbConnection connection);

        /// <summary>
        /// Creates the version table.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        void CreateVersionTable(IDbConnection connection);

        /// <summary>
        /// Inserts the specified version into the version table.
        /// </summary>
        /// <param name="version">The version to insert.</param>
        /// <param name="connection">The database connection.</param>
        void InsertVersion(VersionBase version, IDbConnection connection);

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
        /// <param name="scriptName"></param>
        /// <returns></returns>
        bool HasExecutedScript(VersionBase currentVersion, VersionBase targetVersion, string scriptName);

        /// <summary>
        /// Returns the IEqualityComparer ot use for comparing scripts
        /// </summary>
        /// <returns></returns>
        IEqualityComparer<Script> GetScriptComparer();
    }
}
