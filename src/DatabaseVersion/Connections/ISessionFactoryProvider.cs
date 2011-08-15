using System;
using NHibernate;

namespace DatabaseVersion.Connections
{
    public interface ISessionFactoryProvider
    {
        ISessionFactory CreateSessionFactory();
    }
}

