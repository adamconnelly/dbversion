namespace dbversion
{
    using dbversion.Archives;
    using dbversion.Tasks;

    public interface IDatabaseCreator
    {
        /// <summary>
        /// Creates or upgrades an existing database.
        /// </summary>
        /// <param name="archive">The archive containing the versions.</param>
        /// <param name="version">The target version.</param>
        /// <param name="taskExecuter">The executer.</param>
        /// <param name="commit">true if the changes should be committed, false if they should be rolled back.</param>
        /// <param name="executeMissingTasks">true if any missing tasks should be executed, false if only new tasks should be executed.</param>
        /// <returns>true if successful, otherwise false.</returns>
        bool Create(IDatabaseArchive archive, string version, ITaskExecuter taskExecuter, bool commit, bool executeMissingTasks);
    }
}

