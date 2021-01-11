using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples.KeyVault;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Translate
{
    public static class Translator
    {
        private const string SubscriptionKeySecretName = "translator-key";
        private const string EndpointSecretName = "translator-endpoint";
        private const string Route = "/translate?api-version=3.0&to=fr";
        private const string SubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
        private const string MediaType = "application/json";
        
        private static readonly string SubscriptionKey = SecretProvider.GetSecret(SubscriptionKeySecretName);
        private static readonly string Endpoint = SecretProvider.GetSecret(EndpointSecretName);

        public static async Task<string> TranslateTextRequest(string inputText)
        {
            object[] body = {new {Text = inputText}};
            var requestBody = JsonConvert.SerializeObject(body);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Endpoint + Route),
                Content = new StringContent(requestBody, Encoding.UTF8, MediaType)
            };

            // Build the request.
            request.Headers.Add(SubscriptionKeyHeader, SubscriptionKey);

            // Send the request and get response.
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Read response as a string.
            var result = await response.Content.ReadAsStringAsync();
            var translation = JsonConvert.DeserializeObject<TranslationResult[]>(result).First().Translations.First();
            return translation.Text;
        }
    }
}