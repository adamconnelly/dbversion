namespace DatabaseVersion.Tasks.Version
{
    /// <summary>
    /// Inserts a version into the database.
    /// </summary>
    public class InsertVersionTask : IDatabaseTask
    {
        private readonly IVersionProvider versionProvider;
        private readonly object version;

        public InsertVersionTask(IVersionProvider versionProvider, object version)
        {
            this.versionProvider = versionProvider;
            this.version = version;
        }

        public int ExecutionOrder
        {
            get { return -1; }
        }

        public string Description
        {
            get
            {
                return string.Format("Inserted version \"{0}\"", this.version);
            }
        }

        public void Execute(System.Data.IDbConnection connection)
        {
            this.versionProvider.InsertVersion(version, connection);
        }
    }
}
