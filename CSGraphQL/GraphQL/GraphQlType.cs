using System.Linq;
using System.Text;
using CaseExtensions;
using System.Reflection;
using System.Collections.Generic;
using CSGraphQL.GraphQL.Properties;

namespace CSGraphQL.GraphQL
{
	public abstract class GraphQlType
	{
		public readonly string Name;
		internal TypeField[] TypeFields;

		protected GraphQlType()
		{
			var tp = GetType();
			Name = tp.GetCustomAttribute<TypeNameAttribute>()?.Name ?? tp.Name;
			TypeFields = GetFields(this).ToArray();
		}

		public static IEnumerable<TypeField> GetFields(GraphQlType type)
		{
			var tp = type.GetType();
			var properties = tp.GetProperties();

			return 
				from prop in properties
				let attr = prop.GetCustomAttribute<TypeFieldAttribute>()
				where attr != null
				select new TypeField(prop, attr, type);
		}

		public override string ToString()
		{
			var str = new StringBuilder();

			str.Append(Name);
			if (TypeFields.Length != 0)
			{
				str.Append(" [");
				foreach (var field in TypeFields)
					str.Append($"{field.Name}: {field.ValueType.Name}, ");
				str.Remove(str.Length - 2, 2).Append("]");
			}

			return str.ToString();
		}

		public string AsQueryString(bool root = false, string name = null)
		{
			var str = new StringBuilder();

			if (name != null) str.Append(name); 
			else str.Append(root ? Name : Name.ToCamelCase());
			
			str.AppendLine(" {");

			foreach (var request in TypeFields)
				str.AppendLine(request.ToString());

			str.Append("}");

			return str.ToString();
		}
	}
}