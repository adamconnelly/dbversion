using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace DatabaseVersion
{
    public static class Validate
    {
        public static void NotNull(Expression<Func<object>> paramExpression)
        {
            object value = paramExpression.Compile()();
            string memberName = GetMemberName(paramExpression);
            
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(memberName);
            }
        }

        public static void NotEmpty(Expression<Func<string>> paramExpression)
        {
            string value = paramExpression.Compile()();
            string memberName = GetMemberName(paramExpression);

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Argument cannot be null", memberName);
            }
        }

        private static string GetMemberName<T>(Expression<Func<T>> paramExpression)
        {
            MemberExpression expression = paramExpression.Body as MemberExpression;

            if (expression != null)
            {
                return expression.Member.Name;
            }

            return null;
        }
    }
}
