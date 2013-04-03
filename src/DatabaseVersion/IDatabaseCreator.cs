namespace dbversion
{
    using dbversion.Archives;
    using dbversion.Tasks;

    public interface IDatabaseCreator
    {
        bool Create(IDatabaseArchive archive, string version, ITaskExecuter taskExecuter, bool commit);
    }
}

