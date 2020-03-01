using System.Reflection;
using CSGraphQL.GraphQL;

namespace CSGraphQL.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsGraphQlType(this PropertyInfo info)
            => info.PropertyType.BaseType == typeof(GraphQlType);
        
        public static bool IsGraphQlTypeArray(this PropertyInfo info)
        {
            if (!info.PropertyType.IsArray) return false;
            return info.PropertyType.GetElementType()?.BaseType == typeof(GraphQlType);
        }
        
        public static bool IsGraphQlQuery(this PropertyInfo info)
            => info.PropertyType.BaseType == typeof(GraphQlQuery);
        
        public static bool IsGraphQlQueryArray(this PropertyInfo info)
            => info.PropertyType.IsArray && info.PropertyType.GetElementType().BaseType == typeof(GraphQlQuery);
    }
}