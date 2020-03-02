using System;
using System.IO;
using System.Text;
using CSGraphQL.GraphQL;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSGraphQL.Extensions
{
	public static class ClientExtensions
	{
		public static string PostToJson(this GraphQlClient client, GraphQlQuery query, params KeyValuePair<string, string>[] headers)
			=> PostToJsonAsync(client, query, headers).Result;
		
		public static async Task<string> PostToJsonAsync(this GraphQlClient client, GraphQlQuery query, params KeyValuePair<string, string>[] headers)
		{
			var response = await client.PostQueryAsync(query, headers);

			await using var stream = response?.GetResponseStream();

			using var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
			return reader.ReadToEnd();
		}
		
		public static string PostToJson(this GraphQlClient client, GraphQlMutation mutation, params KeyValuePair<string, string>[] headers)
			=> PostToJsonAsync(client, mutation, headers).Result;
		
		public static async Task<string> PostToJsonAsync(this GraphQlClient client, GraphQlMutation mutation, params KeyValuePair<string, string>[] headers)
		{
			var response = await client.PostMutationAsync(mutation, headers);

			await using var stream = response?.GetResponseStream();

			using var reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
			return reader.ReadToEnd();
		}
	}
}