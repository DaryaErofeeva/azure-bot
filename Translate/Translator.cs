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
        private const string SubscriptionKeySecretName = "translator-endpoint";
        private const string EndpointSecretName = "translator-key";
        private const string Route = "/translate?api-version=3.0&to=fr";
        private const string SubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
        private const string MediaType = "application/json";

        public static async Task<string> TranslateTextRequest(SecretProvider secretProvider, string inputText)
        {
            var subscriptionKey = secretProvider.GetSecret(SubscriptionKeySecretName);
            var endpoint = secretProvider.GetSecret(EndpointSecretName);

            object[] body = {new {Text = inputText}};
            var requestBody = JsonConvert.SerializeObject(body);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint + Route),
                Content = new StringContent(requestBody, Encoding.UTF8, MediaType)
            };

            // Build the request.
            request.Headers.Add(SubscriptionKeyHeader, subscriptionKey);

            // Send the request and get response.
            var response = await client.SendAsync(request).ConfigureAwait(false);

            // Read response as a string.
            var result = await response.Content.ReadAsStringAsync();
            var translation = JsonConvert.DeserializeObject<TranslationResult[]>(result).First().Translations.First();
            return translation.Text;
        }
    }
}