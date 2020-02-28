using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;
using CaseExtensions;
using System;

namespace CSGraphQL.Queries
{
    public class QueryNameAttribute : Attribute
    {
        public readonly string Name;
        public QueryNameAttribute(string name) => Name = name;
    }
    
    public class QueryVariableAttribute : Attribute
    {
        public readonly string Name;

        public QueryVariableAttribute([CallerMemberName] string name = null) 
            => Name = name.ToCamelCase();
    }

    public class QueryRequestAttribute : Attribute
    {
        public readonly string Name;

        public QueryRequestAttribute([CallerMemberName] string name = null) 
            => Name = name.ToCamelCase();
    }
    
    public class TypeNameAttribute : Attribute
    {
        public readonly string Name;
        public TypeNameAttribute(string name) => Name = name;
    }

    public class TypeFieldAttribute : QueryRequestAttribute
    {
        public TypeFieldAttribute([CallerMemberName] string name = null) : base(name) {}
    }
}