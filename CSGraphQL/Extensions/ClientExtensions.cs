using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System;
using CSGraphQL.GraphQL;

namespace CSGraphQL.Client
{
    public static class ClientExtensions
    {
        public static async Task<string> PostToJsonAsync<T>(this GraphQlClient client, Query<T> query, params KeyValuePair<string, string>[] headers) where T : GraphQL.Type
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

        public static string PostToJson<T>(this GraphQlClient client, Query<T> query, params KeyValuePair<string, string>[] headers) where T : GraphQL.Type
            => PostToJsonAsync(client, query, headers).Result;

    }
}