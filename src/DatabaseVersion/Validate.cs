using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DatabaseVersion
{
    public static class Validate
    {
        public static void NotNull(object param, string paramName)
        {
            if (object.ReferenceEquals(param, null))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
