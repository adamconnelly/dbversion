using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

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
            throw new NotImplementedException();
        }

        public object GetLatestVersion(System.Data.IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public void CreateVersionTable(System.Data.IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public void InsertVersion(object version, System.Data.IDbConnection connection)
        {
            throw new NotImplementedException();
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
