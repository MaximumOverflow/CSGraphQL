using System.Collections.Generic;
using System.Threading.Tasks;
using CSGraphQL.Queries;
using System.Text;
using System.IO;
using System;

namespace CSGraphQL.Client
{
    public static class ClientExtensions
    {
        public static async Task<string> PostToJsonAsync(this GraphQlClient client, Query query, params KeyValuePair<string, string>[] headers)
        {
            var response = await client.PostAsync(query, headers);

            await using var stream = response?.GetResponseStream();

            if (stream == null) return null;

            try
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch (Exception) { return null; }
        }

        public static string PostToJson(this GraphQlClient client, Query query, params KeyValuePair<string, string>[] headers)
            => PostToJsonAsync(client, query, headers).Result;

    }
}