using System;
using System.Reflection;
using CSGraphQL.Extensions;

namespace CSGraphQL.GraphQL.Properties
{
	public class TypeField : PropertyPair<TypeFieldAttribute>
	{
		public TypeField(PropertyInfo property, TypeFieldAttribute attribute, object parent) : base(property, attribute, parent) {}

		public bool IsCustomType => Property.IsGraphQlType();
		public bool IsCustomTypeArray => Property.IsGraphQlTypeArray();

		public override string ToString()
		{
			if (IsCustomType) return ((GraphQlType) Activator.CreateInstance(ValueType)).AsQueryString();
			if (IsCustomTypeArray) return ((GraphQlType) Activator.CreateInstance(ValueType.GetElementType())).AsQueryString();

			return Name;
		}
	}
}