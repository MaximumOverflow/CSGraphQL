using System;
using System.Collections.Generic;
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
		
		public bool IsQuery => ValueType.IsSubclassOf(typeof(GraphQlQuery));
		public bool IsQueryArray => ValueType.IsSubclassOf(typeof(IEnumerable<GraphQlQuery>));
		public bool IsCustomType => ValueType.IsSubclassOf(typeof(GraphQlType));
		public bool IsCustomTypeArray => ValueType.IsSubclassOf(typeof(IEnumerable<GraphQlType>));
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
				Enum _ => value.ToString().IsAllUpperCase() ? value.ToString() : value.ToString().ToUpperSnakeCase(),
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