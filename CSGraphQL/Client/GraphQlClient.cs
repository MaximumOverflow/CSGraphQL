using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CSGraphQL.GraphQL;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.IO;
using System;

using Type = CSGraphQL.GraphQL.Type;

namespace CSGraphQL.Client
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

        internal WebResponse PostQuery<T>(Query<T> query, params KeyValuePair<string, string>[] headers) where T : Type
             => PostQueryAsync(query, headers).Result;

        internal async Task<WebResponse> PostQueryAsync<T>(Query<T> query, params KeyValuePair<string, string>[] headers) where T : Type
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
        
        
        public async Task<T> PostAsync<T>(Query<T> query, params KeyValuePair<string, string>[] headers) where T : Type
        {
            var json = (JObject) JObject.Parse(await this.PostToJsonAsync(query, headers))["data"][query.Name];
            return JsonConvert.DeserializeObject<T>(json.ToString(), JsonSettings);
        }

        public T Post<T>(Query<T> query, params KeyValuePair<string, string>[] headers) where T : Type
            => PostAsync<T>(query, headers).Result;
        
        private WebRequest SetupRequest<T>(Query<T> query, params KeyValuePair<string, string>[] headers) where T : Type
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            var json = JsonConvert.SerializeObject(new { query = query.ToString() });
            
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = jsonBytes.Length;
            
            using (var stream = request.GetRequestStream())
                stream.Write(jsonBytes);

            return request;
        }
    }
}