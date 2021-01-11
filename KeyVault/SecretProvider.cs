using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Microsoft.BotBuilderSamples.KeyVault
{
    public static class SecretProvider
    {
        private const string Endpoint = "https://speechbot-keystorage.vault.azure.net/";
        private static readonly SecretClient Client = new SecretClient(new Uri(Endpoint), new DefaultAzureCredential());

        internal static string GetSecret(string secretName)
        {
            return Client.GetSecret(secretName).Value.Value;
        }
    }
}