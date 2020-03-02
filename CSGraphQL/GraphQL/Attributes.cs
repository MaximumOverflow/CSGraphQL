using System;
using System.Runtime.CompilerServices;
using CaseExtensions;

namespace CSGraphQL.GraphQL
{
	public abstract class NamedAttribute : Attribute
	{
		public readonly string Name;
		public readonly bool ExpandContents;
		public NamedAttribute([CallerMemberName] string name = null, bool expandContents = true)
		{
			Name = name;
			ExpandContents = expandContents;
		}
	}
	
	[AttributeUsage(AttributeTargets.Class)]
	public class TypeNameAttribute : NamedAttribute
	{
		public TypeNameAttribute([CallerMemberName] string name = null) : base(name.ToPascalCase()) {}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class TypeFieldAttribute : NamedAttribute
	{
		public TypeFieldAttribute([CallerMemberName] string name = null, bool expandContentsAsQuery = true) : base(name.ToCamelCase(), expandContentsAsQuery) {}
	}

	public enum QueryFieldType { Variable, NestedVariable, Request }

	[AttributeUsage(AttributeTargets.Class)]
	public class QueryNameAttribute : NamedAttribute
	{
		public QueryNameAttribute([CallerMemberName] string name = null) : base(name.ToPascalCase()) {}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class QueryFieldAttribute : NamedAttribute
	{
		
		public readonly QueryFieldType Type;
		public QueryFieldAttribute(QueryFieldType type, [CallerMemberName] string name = null, bool expandContents = true) : base(name.ToCamelCase(), expandContents)
			=> Type = type;

		public bool IsVariable => Type == QueryFieldType.Variable;
		public bool IsRequest => Type == QueryFieldType.Request;
	}
}