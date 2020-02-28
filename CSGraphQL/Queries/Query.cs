using QueryVariable = System.Collections.Generic.KeyValuePair<string, System.Reflection.PropertyInfo>;
using System.Reflection;
using System.Linq;
using System.Text;
using System;

namespace CSGraphQL.Queries
{
    public abstract class Query
    {
        private readonly Type _type;
        private readonly QueryVariable[] _variables;
        private readonly string[] _requests;
        
        public readonly string Name;

        protected Query()
        {
            _type = GetType();
            Name = _type.GetCustomAttribute<QueryNameAttribute>()?.Name ?? _type.Name;

            _variables = GetVariables(this);
            _requests = GetRequests(this);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("{");

            //Query and variables
            str.Append($"{Name}(");

            foreach (var variable in _variables)
                str.Append($"{variable.Key}: {variable.Value.GetValue(this)}, ");

            if(_variables.Length != 0) 
                str.Remove(str.Length-2, 2).AppendLine("){");
            
            //Query requests
            foreach (var request in _requests)
                str.AppendLine(request);
            
            str.AppendLine("}");
            str.AppendLine("}");

            return str.ToString();
        }
        
        private static QueryVariable[] GetVariables(Query query)
        {
            var properties = query._type.GetProperties().Where(p =>
                    p.CustomAttributes.Any(a => a.AttributeType == typeof(QueryVariableAttribute)))
                .ToArray();

            var attributes = 
                properties.Select(p => p.GetCustomAttribute<QueryVariableAttribute>())
                    .ToArray();

            var variables = new QueryVariable[properties.Length];
            for (var i = 0; i < properties.Length; i++)
                variables[i] = new QueryVariable(attributes[i].Name, properties[i]);

            return variables;
        }
        private static string[] GetRequests(Query query)
        {
            var properties = query._type.GetProperties().Where(p =>
                    p.CustomAttributes.Any(a => a.AttributeType == typeof(QueryRequestAttribute)))
                .ToArray();

            var attributes = 
                properties.Select(p => p.GetCustomAttribute<QueryRequestAttribute>())
                    .ToArray();

            return attributes.Select(a => a.Name).ToArray();
        }
    }
}