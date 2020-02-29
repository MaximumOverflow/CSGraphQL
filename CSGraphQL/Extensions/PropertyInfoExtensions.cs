using System.Reflection;
using CSGraphQL.GraphQL;

namespace CSGraphQL.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsCustomGraphQlType(this PropertyInfo info)
            => info.PropertyType.BaseType == typeof(Type);
        
        public static bool IsCustomGraphQlTypeArray(this PropertyInfo info)
        {
            if (!info.PropertyType.IsArray) return false;
            return info.PropertyType.GetElementType()?.BaseType == typeof(Type);
        }

        public static Type GetAsCustomType(this PropertyInfo info, object caller)
            => info.GetValue(caller) as Type;
        
        public static Type[] GetAsCustomTypeArray(this PropertyInfo info, object caller)
            => info.GetValue(caller) as Type[];
    }
}