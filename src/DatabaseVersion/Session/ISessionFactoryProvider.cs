namespace dbversion.Session
{
    using NHibernate;
    using NHibernate.Cfg;

    public interface ISessionFactoryProvider
    {
        ISessionFactory CreateSessionFactory();

        /// <summary>
        /// Gets the configuration with no mappings added.
        /// </summary>
        /// <returns>
        /// The configuration.
        /// </returns>
        Configuration GetConfiguration();
    }
}

