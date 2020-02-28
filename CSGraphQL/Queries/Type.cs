using System.Linq;
using QueryRequest = System.Collections.Generic.KeyValuePair<CSGraphQL.Queries.QueryRequestAttribute, System.Reflection.PropertyInfo>;
using System.Reflection;
using System.Text;

namespace CSGraphQL.Queries
{
    public abstract class Type
    {
        private readonly QueryRequest[] _requests;

        public readonly string Name;

        public Type()
        {
            _requests = GetFields(this);
            var type = GetType();
            Name = type.GetCustomAttribute<TypeNameAttribute>()?.Name ?? type.Name;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"{Name}{{");

            foreach (var request in _requests)
                str.AppendLine(Query.RequestToString(request));

            str.Append("}");

            return str.ToString();
        }
        
        public static QueryRequest[] GetFields(object query)
        {
            var properties = query.GetType().GetProperties().Where(p =>
                    p.CustomAttributes.Any(a => a.AttributeType == typeof(TypeFieldAttribute)))
                .ToArray();

            var attributes = 
                properties.Select(p => p.GetCustomAttribute<TypeFieldAttribute>())
                    .ToArray();

            var variables = new QueryRequest[properties.Length];
            for (var i = 0; i < properties.Length; i++)
                variables[i] = new QueryRequest(attributes[i], properties[i]);

            return variables;
        }
    }
}