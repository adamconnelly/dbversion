using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace DatabaseVersion
{
    /// <summary>
    /// Contains methods for validating parameters passed into methods.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Validates that the parameter is not null.
        /// </summary>
        /// <param name="paramExpression">An expression that can be used to access the parameter.</param>
        /// <example>
        /// <code>
        /// public void OutputString(string output)
        /// {
        ///   Validate.NotNull(() => output);
        ///   
        ///   Console.WriteLine(output);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the parameter is null.
        /// </exception>
        public static void NotNull(Expression<Func<object>> paramExpression)
        {
            object value = paramExpression.Compile()();
            string memberName = GetMemberName(paramExpression);
            
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(memberName);
            }
        }

        /// <summary>
        /// Validates that the parameter is not empty.
        /// </summary>
        /// <param name="paramExpression">An expression that can be used to access the parameter.</param>
        /// <example>
        /// <code>
        /// public void OutputString(string output)
        /// {
        ///   Validate.NotEmpty(() => output);
        ///   
        ///   Console.WriteLine(output);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentException">
        /// Thrown if the parameter is null or empty.
        /// </exception>
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
