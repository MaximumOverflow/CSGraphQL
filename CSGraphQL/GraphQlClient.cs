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

using Header = System.Collections.Generic.KeyValuePair<string, string>;

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

        internal async Task<WebResponse> PostRequestAsync(WebRequest request, params Header[] headers)
        {
            try { return await request.GetResponseAsync(); }
            catch (WebException e)
            {
                using var stream = new StreamReader(e.Response.GetResponseStream() ?? throw e);
                Console.WriteLine(stream.ReadToEnd());
                throw;
            }
        }

        internal async Task<WebResponse> PostQueryAsync(GraphQlQuery query, params Header[] headers)
            => await PostRequestAsync(SetupQuery(query, headers), headers);
        internal async Task<WebResponse> PostMutationAsync(GraphQlMutation mutation, params Header[] headers)
            => await PostRequestAsync(SetupMutation(mutation, headers), headers);


        public T Post<T>(GraphQlQuery query, params Header[] headers) where T : GraphQlType
            => PostAsync<T>(query, headers).Result;
        
        public async Task<T> PostAsync<T>(GraphQlQuery query, params Header[] headers) where T : GraphQlType
        {
            var json = (JObject) JObject.Parse(await this.PostToJsonAsync(query, headers))["data"][query.Name];
            return JsonConvert.DeserializeObject<T>(json.ToString(), JsonSettings);
        }
        
        public T Post<T>(GraphQlMutation mutation, params Header[] headers) where T : GraphQlType
            => PostAsync<T>(mutation, headers).Result;
        
        public async Task<T> PostAsync<T>(GraphQlMutation mutation, params Header[] headers) where T : GraphQlType
        {
            var json = (JObject) JObject.Parse(await this.PostToJsonAsync(mutation, headers))["data"][mutation.Name];
            return JsonConvert.DeserializeObject<T>(json.ToString(), JsonSettings);
        }

        private WebRequest SetupRequest(string json, params Header[] headers)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
            
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = jsonBytes.Length;
            
            using (var stream = request.GetRequestStream())
                stream.Write(jsonBytes);

            return request;
        }
        
        private WebRequest SetupQuery(GraphQlQuery query, params Header[] headers)
        {
            var json = JsonConvert.SerializeObject(new { query = $"query {{\n{query}\n}}" });
            return SetupRequest(json, headers);
        }
        
        private WebRequest SetupMutation(GraphQlMutation mutation, params Header[] headers)
        {
            var json = JsonConvert.SerializeObject(new { query = $"mutation {{\n{mutation.ToString(true)}\n}}" });
            return SetupRequest(json, headers);
        }
    }
}