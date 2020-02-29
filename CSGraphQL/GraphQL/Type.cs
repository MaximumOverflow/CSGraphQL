using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CSGraphQL.Extensions;
using Variable = System.Tuple<CSGraphQL.GraphQL.VariableAttribute, System.Reflection.PropertyInfo>;
using Request = System.Tuple<CSGraphQL.GraphQL.RequestAttribute, System.Reflection.PropertyInfo>;

namespace CSGraphQL.GraphQL
{
    public abstract class Type
    {
        protected internal readonly Variable[] Variables;
        protected internal readonly Request[] Requests;

        public Type()
        {
            Variables = GetPropertyPair<VariableAttribute>(this);
            Requests = GetPropertyPair<RequestAttribute>(this);

            foreach (var (_, property) in Requests.Where(r => r.Item2.IsCustomGraphQlType()))        //Create custom type instances
                property.SetValue(this, Activator.CreateInstance(property.PropertyType));

            foreach (var (_, property) in Requests.Where(r => r.Item2.IsCustomGraphQlTypeArray()))    //Create custom type array instances
            {
                property.SetValue(this, Activator.CreateInstance(property.PropertyType, 1));
                var array = property.GetAsCustomTypeArray(this);
                array[0] = Activator.CreateInstance(property.PropertyType.GetElementType() ?? throw new Exception()) as Type;
            }
        }

        public IEnumerable<Tuple<object, IEnumerable<Variable>>> GetVariablesRecursive()
        {
            var vars = new List<Tuple<object, IEnumerable<Variable>>>(new [] { new Tuple<object, IEnumerable<Variable>>(this, Variables)});
            
            var typeVars = new List<Type>();
            
            typeVars.AddRange(Variables
                .Where(v => v.Item2.IsCustomGraphQlType())
                .Select(v => v.Item2.GetAsCustomType(this)));
            
            typeVars.AddRange(Requests
                .Where(v => v.Item2.IsCustomGraphQlType())
                .Select(v => v.Item2.GetAsCustomType(this)));
            
            typeVars.AddRange(Requests.
                Where(v => v.Item2.IsCustomGraphQlTypeArray())
                .Select(v => v.Item2.GetAsCustomTypeArray(this)[0]));

            foreach (var var in typeVars)
                vars.AddRange(var?.GetVariablesRecursive());

            return vars;
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            var nonNullVariables = Variables
                .Where(v => v.Item2.GetValue(this) != null)
                .ToArray(); 
            
            if (nonNullVariables.Length != 0)
            {
                str.Append("(");
                foreach (var variable in Variables)
                    str.Append(VariableToString(variable));
                str.Remove(str.Length - 2, 2).Append(")");
            }

            str.AppendLine("{");

            foreach (var (attribute, propertyInfo) in Requests)
            {
                str.Append(attribute.Name);

                if(propertyInfo.IsCustomGraphQlType())             //Custom GraphQL Type
                    str.Append(propertyInfo.GetValue(this));
                if(propertyInfo.IsCustomGraphQlTypeArray())        //Custom GraphQL Type array
                {
                    var instance = propertyInfo.GetAsCustomTypeArray(this)[0];
                    str.Append(instance);
                }
                    
                str.AppendLine();
            }

            str.Append("}");
            return str.ToString();
        }

        private string VariableToString(Variable variable)
        {
            var name = variable.Item1.Name;
            var value = variable.Item2.GetValue(this);
            
            if (value == null) return null;

            var str = value switch
            {
                string _ => $"{name}: \"{value}\"",
                bool _ => $"{name}: {value.ToString().ToLower()}",
                _ => $"{name}: {value}"
            };

            return str + ", ";
        }

        private static Tuple<T, PropertyInfo>[] GetPropertyPair<T>(Type type) where T : Attribute
        {
            var tp = type.GetType();
            var properties = tp.GetProperties();
            
            var list = new List<Tuple<T, PropertyInfo>>(properties.Length);
            list.AddRange(
                from property in properties 
                let attribute = property.GetCustomAttribute<T>() 
                where attribute != null select new Tuple<T, PropertyInfo>(attribute, property)
            );

            return list.ToArray();
        }
    }
}