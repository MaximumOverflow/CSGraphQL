using System;
using System.Reflection;
using System.Text;
using CaseExtensions;
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
				return $"{Name}: {ValueToString(Value)}";
			}

			if (!Attribute.ExpandContents) return Name;
			
			if (IsQuery) return ValueAsQuery.ToString();
			if (IsQueryArray) return Activator.CreateInstance(ValueType.GetElementType()).ToString();

			if (IsCustomType) return ((GraphQlType) Activator.CreateInstance(ValueType)).AsQueryString(name: Name);
			if (IsCustomTypeArray) return ((GraphQlType) Activator.CreateInstance(ValueType.GetElementType())).AsQueryString();
			return Name;
		}

		private string ValueToString(object value)
		{
			if (value.GetType().IsArray) return ArrayToString(value);
			
			return value switch
			{
				Enum _ => value.ToString().ToSnakeCase().ToUpper(),
				string _ => $"\"{value}\"",
				bool _ => value.ToString().ToLower(),
				_ => value.ToString()
			};
		}

		private string ArrayToString(dynamic array)
		{
			var str = new StringBuilder();

			str.Append("[");
			if (array.Length != 0)
			{
				foreach (var value in array)
					str.Append(ValueToString(value)).Append(", ");
				str.Remove(str.Length - 2, 2);
			}
			str.Append("]");

			return str.ToString();
		}
	}
}