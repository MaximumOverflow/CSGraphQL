using System;
using QueryVariable = System.Collections.Generic.KeyValuePair<string, System.Reflection.PropertyInfo>;
using QueryRequest = System.Collections.Generic.KeyValuePair<CSGraphQL.Queries.QueryRequestAttribute, System.Reflection.PropertyInfo>;
using System.Reflection;
using System.Linq;
using System.Text;

namespace CSGraphQL.Queries
{
    public abstract class Query
    {
        private readonly System.Type _type;
        private readonly QueryVariable[] _variables;
        private readonly QueryRequest[] _requests;
        
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
            {
                var name = variable.Key;
                var value = variable.Value.GetValue(this);
                if (value != null)
                    str.Append(value is string ? $"{name}: \"{value}\", " : $"{name}: {value}, ");
            }

            if (_variables.Length != 0)
                str.Remove(str.Length - 2, 2);
            
            str.AppendLine("){");
            
            //Query requests
            foreach (var request in _requests)
                str.AppendLine(RequestToString(request));
            
            str.AppendLine("}");
            str.AppendLine("}");

            return str.ToString();
        }

        public static string RequestToString(QueryRequest request)
        {
            if (request.Value.PropertyType.BaseType != typeof(Type)) return request.Key.Name;

            var type = Activator.CreateInstance(request.Value.PropertyType) as Type;
            return type != null ? $"{request.Key.Name}{type.GetRequests()}" : null;
        }
        
        public static QueryVariable[] GetVariables(object query)
        {
            var properties = query.GetType().GetProperties().Where(p =>
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
        public static QueryRequest[] GetRequests(object query)
        {
            var properties = query.GetType().GetProperties().Where(p =>
                    p.CustomAttributes.Any(a => a.AttributeType == typeof(QueryRequestAttribute)))
                .ToArray();

            var attributes = 
                properties.Select(p => p.GetCustomAttribute<QueryRequestAttribute>())
                    .ToArray();

            var variables = new QueryRequest[properties.Length];
            for (var i = 0; i < properties.Length; i++)
                variables[i] = new QueryRequest(attributes[i], properties[i]);

            return variables;
        }
    }
}