﻿using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System;
using System.Data;
namespace DatabaseVersion.Connections
{
    [Export(typeof(IDbConnectionCreator))]
    public class SqlServerConnectionCreator : IDbConnectionCreator
    {
        public string ConnectionType
        {
            get
            {
                return "SqlServer";
            }
        }

        public IDbConnection Create(string connectionString)
        {
            try
            {
                return new SqlConnection(connectionString);
            }
            catch (Exception)
            {
                throw new UnsupportedConnectionStringException(connectionString);
            }
        }
    }
}
