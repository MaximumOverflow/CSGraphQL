using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CaseExtensions;
using CSGraphQL.GraphQL.Properties;

namespace CSGraphQL.GraphQL
{
	public abstract class GraphQlMutation
	{
		internal QueryField[] Requests;
		internal QueryField[] Variables;
		internal QueryField[] NestedVariables;
		
		internal string Name;

		protected GraphQlMutation()
		{
			var tp = GetType();
			Name = tp.GetCustomAttribute<MutationNameAttribute>()?.Name ?? tp.Name;

			Requests = GetFields(this, QueryFieldType.Request).ToArray();
			Variables = GetFields(this, QueryFieldType.Variable).ToArray();
			NestedVariables = GetFields(this, QueryFieldType.NestedVariable).ToArray();

			foreach (var query in Requests.Where(r => r.IsQuery))
				query.Value = Activator.CreateInstance(query.ValueType);
		}

		private void SetVariables(QueryField[] variables)
		{
			foreach (var variable in variables)
			{
				try { Variables.First(v => v.Name == variable.Name).Value = variable.Value; }
				catch (InvalidOperationException) {}
			}
		}

		private void SetVariablesRecursive(QueryField[] variables)
		{
			SetVariables(variables);
			foreach (var request in Requests.Where(r => r.IsQuery))
				request.ValueAsQuery.SetVariablesRecursive(NestedVariables);
		}

		public override string ToString() => ToString(false);
		public virtual string ToString(bool root)
		{
			SetVariablesRecursive(NestedVariables);
			
			var str = new StringBuilder();

			str.Append(root ? Name : Name.ToCamelCase());
			
			var nonNullVars = Variables.Where(v => v.Value != null).ToArray();
			if (nonNullVars.Length != 0)
			{
				str.Append("(");
				foreach (var variable in nonNullVars)
					str.Append(variable).Append(", ");
				str.Remove(str.Length - 2, 2);
				str.Append(")");
			}

			str.AppendLine(" {");

			foreach (var request in Requests)
				str.AppendLine(request.ToString());

			str.Append("}");

			return str.ToString();
		}

		public static IEnumerable<QueryField> GetFields(GraphQlMutation query, QueryFieldType type)
		{
			var properties = query.GetType().GetProperties();

			return
				from prop in properties
				let attr = prop.GetCustomAttribute<QueryFieldAttribute>()
				where attr != null && attr.Type == type
				select new QueryField(prop, attr, query, attr.Type);
		}
	}
}