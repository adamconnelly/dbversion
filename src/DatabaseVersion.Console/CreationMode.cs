namespace DatabaseVersion.Console
{
    public enum CreationMode
    {
        /// <summary>
        /// Creates a new database from scratch.
        /// </summary>
        Create,

        /// <summary>
        /// Upgrades an existing database.
        /// </summary>
        Upgrade
    }
}
