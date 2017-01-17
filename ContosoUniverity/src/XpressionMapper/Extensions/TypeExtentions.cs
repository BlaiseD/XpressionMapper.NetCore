using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XpressionMapper.Extensions
{
    internal static class TypeExtentions
    {
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            MethodInfo methodInfo = null;
            PropertyInfo propertyInfo = null;
            FieldInfo fieldInfo = null;
            if ((methodInfo = memberInfo as MethodInfo) != null)
                return methodInfo.ReturnType;
            if ((propertyInfo = memberInfo as PropertyInfo) != null)
                return propertyInfo.PropertyType;
            if ((fieldInfo = memberInfo as FieldInfo) != null)
                return fieldInfo.FieldType;
            return null;
        }
    }
}
