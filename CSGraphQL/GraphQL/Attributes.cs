using System;
using System.Runtime.CompilerServices;
using CaseExtensions;

namespace CSGraphQL.GraphQL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueryNameAttribute : Attribute
    {
        public readonly string Name;
        public QueryNameAttribute(string name) => Name = name.ToCamelCase();
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeNameAttribute : Attribute
    {
        public readonly string Name;
        public TypeNameAttribute(string name) => Name = name.ToCamelCase();
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class VariableAttribute : Attribute
    {
        public readonly string Name;
        public VariableAttribute([CallerMemberName] string name = null) => Name = name.ToCamelCase();
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class RequestAttribute : Attribute
    {
        public readonly string Name;
        public RequestAttribute([CallerMemberName] string name = null) => Name = name.ToCamelCase();
    }
}