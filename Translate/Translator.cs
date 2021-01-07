using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Translate
{
    public class Translator
    {
        static public async Task<string> TranslateTextRequest(string subscriptionKey, string endpoint, string route,
            string inputText)
        {
            object[] body = new object[] {new {Text = inputText}};
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                Translation translation = JsonConvert.DeserializeObject<TranslationResult[]>(result).First()
                    .Translations.First();
                return translation.Text;
            }
        }
    }
}