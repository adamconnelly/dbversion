using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Data;

namespace DatabaseVersion.Version
{
    [Export(typeof(IVersionProvider))]
    public class NumericVersionProvider : IVersionProvider
    {
        public object CreateVersion(string versionString)
        {
            return int.Parse(versionString);
        }

        public bool VersionTableExists(System.Data.IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "select count(1) from information_schema.tables where table_name = 'version'";
                var count = (int)command.ExecuteScalar();

                return count == 1;
            }
        }

        public object GetCurrentVersion(System.Data.IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "select max(version) from version";

                object version = command.ExecuteScalar();

                if (version == DBNull.Value)
                {
                    return null;
                }

                return (int)version;
            }
        }

        public void CreateVersionTable(System.Data.IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "create table version(version int primary key, updatedOn datetime not null)";
                command.ExecuteNonQuery();
            }
        }

        public void InsertVersion(object version, System.Data.IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = string.Format("insert into version(version, updatedOn) values({0}, @updatedOn)", version);
                IDbDataParameter parameter = command.CreateParameter();
                parameter.DbType = DbType.DateTime;
                parameter.ParameterName = "updatedOn";
                parameter.Value = DateTime.UtcNow;

                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }

        public IComparer<object> GetComparer()
        {
            return new IntComparer();
        }

        private class IntComparer : Comparer<object>
        {
            public override int Compare(object x, object y)
            {
                int intX = (int)x;
                int intY = (int)y;

                return intX - intY;
            }
        }
    }
}
