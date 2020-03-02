using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace CSGraphQL.GraphQL.Short
{
	public class FieldAttribute : TypeFieldAttribute 
	{ public FieldAttribute([CallerMemberName] string name = null, bool expandContentsAsQuery = true) : base(name, expandContentsAsQuery) {} }
		
	public class VariableAttribute : QueryFieldAttribute 
	{ public VariableAttribute([CallerMemberName] string name = null) : base(QueryFieldType.Variable, name) {} }
	
	public class NestedVariableAttribute : QueryFieldAttribute 
	{ public NestedVariableAttribute([CallerMemberName] string name = null) : base(QueryFieldType.NestedVariable, name) {} }
		
	public class RequestAttribute : QueryFieldAttribute 
	{ public RequestAttribute([CallerMemberName] string name = null, bool expandContents = true) : base(QueryFieldType.Request, name, expandContents) {} }
}