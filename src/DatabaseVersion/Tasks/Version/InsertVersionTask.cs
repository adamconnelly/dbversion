using System;
using dbversion.Version;
using NHibernate;
namespace dbversion.Tasks.Version
{
    /// <summary>
    /// Inserts a version into the database.
    /// </summary>
    public class InsertVersionTask : IDatabaseTask
    {
        private readonly IVersionProvider versionProvider;
        private readonly VersionBase version;

        public InsertVersionTask(IVersionProvider versionProvider, VersionBase version)
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
                return string.Format("Inserting version \"{0}\"", this.version);
            }
        }

        public string FileName
        {
            get { return string.Empty; }
        }

        public void Execute(ISession session, IMessageService messageService)
        {
            DateTime startTime = DateTime.Now;
            messageService.WriteLine(String.Format("Starting Task: {0}", Description));

            this.versionProvider.InsertVersion(version, session);

            messageService.WriteLine(String.Format("Finished Task: {0}. Time Taken: {1}", Description, DateTime.Now.Subtract(startTime)));
        }
    }
}
