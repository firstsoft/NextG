using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LWIR.NET.Utility
{
    /// <summary>
    /// get the name of variable
    /// </summary>
    public static class NameOf
    {
        /// <summary>
        /// var name = ac.nameof(b => b.B); ac is a class, b is a temp variable and type is ac
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TT"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyAccessor"></param>
        /// <returns></returns>
        public static string nameof<T, TT>(this T obj, Expression<Func<T, TT>> propertyAccessor)
        {
            return nameof(propertyAccessor.Body);
        }

        private static string nameof(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null)
                    return null;

                return memberExpression.Member.Name;
            }
            return null;
        }
    }
}
