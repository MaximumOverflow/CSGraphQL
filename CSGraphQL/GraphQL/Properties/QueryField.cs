using System;
using System.Reflection;
using CSGraphQL.Extensions;

namespace CSGraphQL.GraphQL.Properties
{
	public class QueryField : PropertyPair<QueryFieldAttribute>
	{
		public readonly QueryFieldType Type;

		public QueryField(PropertyInfo property, QueryFieldAttribute attribute, object parent, QueryFieldType type)
			: base(property, attribute, parent) => Type = type;
		
		public bool IsQuery => Property.IsGraphQlQuery();
		public bool IsQueryArray => Property.IsGraphQlQueryArray();
		
		public bool IsCustomType => Property.IsGraphQlType();
		public bool IsCustomTypeArray => Property.IsGraphQlTypeArray();
		public GraphQlQuery ValueAsQuery => Value as GraphQlQuery;

		public override string ToString()
		{
			if (Type == QueryFieldType.Variable)
			{
				return Value switch
				{
					Enum _ => $"{Name}: {Value.ToString().ToUpper()}",
					string _ => $"{Name}: \"{Value}\"",
					bool _ => $"{Name}: {Value.ToString().ToLower()}",
					
					_ => $"{Name}: {Value}"
				};
			}
			
			if (IsQuery) return ValueAsQuery.ToString();
			if (IsQueryArray) return Activator.CreateInstance(ValueType.GetElementType()).ToString();

			if (IsCustomType) return ((GraphQlType) Activator.CreateInstance(ValueType)).AsQueryString();
			if (IsCustomTypeArray) return ((GraphQlType) Activator.CreateInstance(ValueType.GetElementType())).AsQueryString();
			return Name;
		}
	}
}