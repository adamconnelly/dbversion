namespace DatabaseVersion.Connections
{
    using System.Data;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    [Export(typeof(IDbConnectionFactory))]
    public class DbConnectionFactory : IDbConnectionFactory
    {
        [ImportMany]
        public IEnumerable<IDbConnectionCreator> Creators { get; set; }

        public IDbConnection Create(string connectionString)
        {
            var creator = this.Creators.FirstOrDefault(c => c.CanCreate(connectionString));

            if (creator == null)
            {
                throw new UnsupportedConnectionStringException(connectionString);
            }

            return creator.Create(connectionString);
        }
    }
}
