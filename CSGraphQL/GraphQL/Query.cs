using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CaseExtensions;

using QueryEntry = System.Tuple<string, CSGraphQL.GraphQL.Type>;
using Variable = System.Tuple<CSGraphQL.GraphQL.VariableAttribute, System.Reflection.PropertyInfo>;
using Request = System.Tuple<CSGraphQL.GraphQL.RequestAttribute, System.Reflection.PropertyInfo>;

namespace CSGraphQL.GraphQL
{
    public abstract class Query<T> : Type where T : Type
    {
        public readonly string Name;
        protected readonly T Type;
        internal readonly IEnumerable<Tuple<object, IEnumerable<Variable>>> NestedVariables;

        protected Query() : this(Activator.CreateInstance<T>()) {}
        protected Query(T type)
        {
            Type = type;
            Name = (GetType().GetCustomAttribute<QueryNameAttribute>()?.Name ?? Type.GetType().Name).ToPascalCase();
            NestedVariables = Type.GetVariablesRecursive().ToArray();
        }

        public override string ToString()
        {
            SetNestedVariables();
            
            var str = new StringBuilder();
            str.AppendLine("{");
            str.Append(Name);
            str.Append(Type).AppendLine();
            str.AppendLine("}");
    
            return str.ToString();
        }

        private void SetNestedVariables()
        {
            foreach (var (obj, objvars) in NestedVariables)
            {
                foreach (var (vattr, vprop) in objvars)
                {
                    try
                    {
                        var qvar = Variables.First(v => v.Item1.Name == vattr.Name);
                        vprop.SetValue(obj, qvar.Item2.GetValue(this));
                    }
                    catch (InvalidOperationException) {}
                }
            }
        }
    }
}