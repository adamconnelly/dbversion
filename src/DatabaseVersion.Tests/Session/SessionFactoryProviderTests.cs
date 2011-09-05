namespace DatabaseVersion.Tests.Session
{
    using System.Linq;

    using dbversion.Session;

    using Xunit;

    public class SessionFactoryProviderTests
    {
        [Fact]
        public void ShouldReturnDefaultPropertiesForSqlServer()
        {
            // Arrange
            var provider = new SessionFactoryProvider();

            // Act
            var defaults = provider.DefaultProperties;

            // Assert
            Assert.Equal("NHibernate.Connection.DriverConnectionProvider", defaults.Single(p => p.Key == "hibernate.connection.provider").Value);
            Assert.Equal("NHibernate.Driver.SqlClientDriver", defaults.Single(p => p.Key == "hibernate.connection.driver_class").Value);
            Assert.Equal("NHibernate.Dialect.MsSql2008Dialect", defaults.Single(p => p.Key == "hibernate.dialect").Value);
        }
    }
}

