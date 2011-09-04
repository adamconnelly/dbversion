namespace dbversion
{
    using dbversion.Archives;
    using dbversion.Tasks;

    public interface IDatabaseCreator
    {
        void Create(IDatabaseArchive archive, string version, ITaskExecuter taskExecuter);
    }
}

