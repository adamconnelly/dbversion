using System;
using dbversion.Version;
using NHibernate;
namespace dbversion.Tasks.Version
{
    /// <summary>
    /// Inserts a version into the database.
    /// </summary>
    public class InsertVersionTask : BaseTask
    {
        private readonly IVersionProvider _versionProvider;
        private readonly VersionBase _version;

        public InsertVersionTask(IVersionProvider versionProvider, VersionBase version, IMessageService messageService)
            : base(String.Empty, -1, messageService)
        {
            this._versionProvider = versionProvider;
            this._version = version;
        }

        protected override string GetTaskDescription()
        {
            return string.Format("Inserting version \"{0}\"", this._version);
        }

        protected override void ExecuteTask(ISession session)
        {
            this._versionProvider.InsertVersion(_version, session);
        }
    }
}
