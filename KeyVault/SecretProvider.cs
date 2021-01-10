using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Microsoft.BotBuilderSamples.KeyVault
{
    public class SecretProvider
    {
        private const string Endpoint = "https://speechbot-keystorage.vault.azure.net/";
        private readonly SecretClient _client = new SecretClient(new Uri(Endpoint), new DefaultAzureCredential());

        internal string GetSecret(string secretName)
        {
            return _client.GetSecret(secretName).Value.Value;
        }
    }
}