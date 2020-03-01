using System;
using System.Reflection;

namespace CSGraphQL.GraphQL.Properties
{
    public class PropertyPair<T> where T : NamedAttribute
    {
        public readonly T Attribute;
        public readonly object Parent;
        public readonly Type ValueType;
        public readonly Type AttributeType;
        public readonly PropertyInfo Property;

        public PropertyPair(PropertyInfo property, T attribute, object parent)
        {
            Parent = parent;
            Property = property;
            Attribute = attribute;
            ValueType = Property.PropertyType; 
            AttributeType = Attribute.GetType();
        }

        public string Name => Attribute.Name;

        public object Value
        {
            get => Property.GetValue(Parent);
            set => Property.SetValue(Parent, value);
        }
    }
}