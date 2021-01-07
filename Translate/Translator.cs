using System;
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
            string responseText = "";

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
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input languge and confidence score.
                    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n",
                        o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    // Iterate over the results and print each translation.
                    foreach (Translation t in o.Translations)
                    {
                        responseText += String.Format("Translated to {0}: {1}", t.To, t.Text);
                    }
                }
            }
            
            return responseText;
        }
    }
}