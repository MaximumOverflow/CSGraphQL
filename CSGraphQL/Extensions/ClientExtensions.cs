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
		public static async Task<string> PostToJsonAsync(this GraphQlClient client, GraphQlQuery query, params KeyValuePair<string, string>[] headers)
		{
			var response = await client.PostQueryAsync(query, headers);

			await using var stream = response?.GetResponseStream();

			if (stream == null) return null;

			try
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
					return reader.ReadToEnd();
			}
			catch (Exception) { return null; }
		}

		public static string PostToJson(this GraphQlClient client, GraphQlQuery query, params KeyValuePair<string, string>[] headers)
			=> PostToJsonAsync(client, query, headers).Result;
	}
}