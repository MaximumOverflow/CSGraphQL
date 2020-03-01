using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using CSGraphQL.GraphQL;
using Newtonsoft.Json.Linq;
using CSGraphQL.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSGraphQL
{
    public class GraphQlClient
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        
        public readonly string Url;
        
        public GraphQlClient(string url) => Url = url;

        internal WebResponse PostQuery(GraphQlQuery query, params KeyValuePair<string, string>[] headers)
             => PostQueryAsync(query, headers).Result;

        internal async Task<WebResponse> PostQueryAsync(GraphQlQuery query, params KeyValuePair<string, string>[] headers)
        {
            var request = SetupRequest(query, headers);

            try { return await request.GetResponseAsync(); }
            catch (WebException e)
            {
                using var stream = new StreamReader(e.Response.GetResponseStream() ?? throw e);
                Console.WriteLine(stream.ReadToEnd());
                throw;
            }
        }
        
        
        public async Task<T> PostAsync<T>(GraphQlQuery query, params KeyValuePair<string, string>[] headers) where T : GraphQlType
        {
            var json = (JObject) JObject.Parse(await this.PostToJsonAsync(query, headers))["data"][query.Name];
            return JsonConvert.DeserializeObject<T>(json.ToString(), JsonSettings);
        }

        public T Post<T>(GraphQlQuery query, params KeyValuePair<string, string>[] headers) where T : GraphQlType
            => PostAsync<T>(query, headers).Result;
        
        private WebRequest SetupRequest(GraphQlQuery query, params KeyValuePair<string, string>[] headers)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            var json = JsonConvert.SerializeObject(new { query = $"query {{\n{query.ToString(true)}\n}}" });
            
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = jsonBytes.Length;
            
            using (var stream = request.GetRequestStream())
                stream.Write(jsonBytes);

            return request;
        }
    }
}