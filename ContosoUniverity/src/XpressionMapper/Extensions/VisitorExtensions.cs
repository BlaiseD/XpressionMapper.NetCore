using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XpressionMapper.Extensions
{
    internal static class VisitorExtensions
    {
        /// <summary>
        /// Returns true if the expression is a direct or descendant member expression of the parameter.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsMemberExpression(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)expression;
                return IsMemberOrParameterExpression(memberExpression.Expression);
            }

            return false;
        }

        private static bool IsMemberOrParameterExpression(Expression expression)
        {
            //the node represents parameter of the expression
            if (expression.NodeType == ExpressionType.Parameter)
                return true;

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)expression;
                return IsMemberOrParameterExpression(memberExpression.Expression);
            }

            return false;
        }

        /// <summary>
        /// Returns the fully qualified name of the member starting with the immediate child member of the parameter
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyFullName(this Expression expression)
        {
            const string PERIOD = ".";

            //the node represents parameter of the expression
            if (expression.NodeType == ExpressionType.Parameter)
                return string.Empty;

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExpression = (MemberExpression)expression;
                string parentFullName = memberExpression.Expression.GetPropertyFullName();
                return string.IsNullOrEmpty(parentFullName)
                    ? memberExpression.Member.Name
                    : string.Concat(memberExpression.Expression.GetPropertyFullName(), PERIOD, memberExpression.Member.Name);
            }

            throw new InvalidOperationException(Resource.invalidExpErr);
        }

        private static MemberExpression GetMemberExpression(LambdaExpression expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    if (me == null)
                    {
                        var binaryExpression = expr.Body as BinaryExpression;
                        if (binaryExpression != null)
                        {
                            MemberExpression left = binaryExpression.Left as MemberExpression;
                            if (left != null)
                                return left;
                            MemberExpression right = binaryExpression.Right as MemberExpression;
                            if (right != null)
                                return right;
                        }
                    }
                    break;
            }

            return me;
        }

        /// <summary>
        /// Returns the ParameterExpression for the LINQ parameter.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ParameterExpression GetParameterExpression(this Expression expression)
        {
            if (expression == null)
                return null;

            //the node represents parameter of the expression
            if (expression.NodeType == ExpressionType.Parameter)
                return (ParameterExpression)expression;

            if (expression.NodeType == ExpressionType.Quote)
            {
                return GetParameterExpression(GetMemberExpression((LambdaExpression)((UnaryExpression)expression).Operand));
            }

            if (expression.NodeType == ExpressionType.Lambda)
            {
                return GetParameterExpression(GetMemberExpression((LambdaExpression)expression));
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExpression = (MemberExpression)expression;
                return GetParameterExpression(memberExpression.Expression);
            }

            if (expression.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodExpression = expression as MethodCallExpression;
                MemberExpression memberExpression = methodExpression.Object as MemberExpression;//Method is an instance method

                bool isExtension = methodExpression.Method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), true);
                if (isExtension && memberExpression == null && methodExpression.Arguments.Count > 0)
                    memberExpression = methodExpression.Arguments[0] as MemberExpression;//Method is an extension method based on the type of methodExpression.Arguments[0] and methodExpression.Arguments[0] is a member expression.

                if (isExtension && memberExpression == null && methodExpression.Arguments.Count > 0)
                    return GetParameterExpression(methodExpression.Arguments[0]);//Method is an extension method based on the type of methodExpression.Arguments[0] but methodExpression.Arguments[0] is not a member expression.
                else
                    return memberExpression == null ? null : GetParameterExpression(memberExpression.Expression);
            }

            return null;
        }

        /// <summary>
        /// Builds and new member expression for the given parameter given its Type and fullname
        /// </summary>
        /// <param name="newParameter"></param>
        /// <param name="type"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static MemberExpression BuildExpression(this ParameterExpression newParameter, Type type, string fullName)
        {
            var parts = fullName.Split('.');

            Expression parent = newParameter;
            foreach (var part in parts)
            {
                PropertyInfo info = type.GetTypeInfo().GetProperty(part);
                parent = Expression.Property(parent, info);
                type = info.PropertyType;
            }

            return (MemberExpression)parent;
        }

        /// <summary>
        /// For the given a Lambda Expression, returns the fully qualified name of the member starting with the immediate child member of the parameter
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetMemberFullName(this LambdaExpression expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            return me.GetPropertyFullName();
        }

        /// <summary>
        /// Returns the underlying type typeof(T) when the type implements IEnumerable<T>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnderlyingGenericType(this Type type)
        {
            if (type == null)
                return null;

            List<Type> types = type.GetTypeInfo().GetInterfaces()
                .Where(t => t.GetTypeInfo().IsGenericType == true && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).ToList();

            return types.Count > 0 ? types[0].GetTypeInfo().GetGenericArguments()[0] : null;
        }
    }
}
