using System.Threading.Tasks;
using Microsoft.BotBuilderSamples.KeyVault;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Microsoft.BotBuilderSamples.Speech
{
    public static class Speech
    {
        private const string SubscriptionKeySecretName = "speech-key";
        private const string RegionSecretName = "speech-location";
        private const string FileName = "outputaudio.mp3";
        private const string Language = "fr-FR";

        private static readonly string SubscriptionKey = SecretProvider.GetSecret(SubscriptionKeySecretName);
        private static readonly string Region = SecretProvider.GetSecret(RegionSecretName);

        internal static async Task<SpeechSynthesisResult> SynthesizeAudioAsync(string translatedText)
        {
            var config = SpeechConfig.FromSubscription(SubscriptionKey, Region);
            config.SpeechSynthesisLanguage = Language;
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);

            using var fileOutput = AudioConfig.FromWavFileInput(FileName);
            using var synthesizer = new SpeechSynthesizer(config, fileOutput);
            var speechResult = await synthesizer.SpeakTextAsync(translatedText);
            return speechResult;
        }
    }
}