using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace CSGraphQL.GraphQL.Short
{
	public class FieldAttribute : TypeFieldAttribute 
	{ public FieldAttribute([CallerMemberName] string name = null) : base(name) {} }
		
	public class VariableAttribute : QueryFieldAttribute 
	{ public VariableAttribute([CallerMemberName] string name = null) : base(QueryFieldType.Variable, name) {} }
	
	public class NestedVariableAttribute : QueryFieldAttribute 
	{ public NestedVariableAttribute([CallerMemberName] string name = null) : base(QueryFieldType.NestedVariable, name) {} }
		
	public class RequestAttribute : QueryFieldAttribute 
	{ public RequestAttribute([CallerMemberName] string name = null) : base(QueryFieldType.Request, name) {} }
}