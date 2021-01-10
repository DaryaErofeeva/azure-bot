using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Microsoft.BotBuilderSamples.Speech
{
    public class Speech
    {
        internal static readonly string FileName = "outputaudio.mp3";

        internal static async Task<SpeechSynthesisResult> SynthesizeAudioAsync(string translatedText)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription("14a1f16db7d349128fb721d73e913ca6", "eastus");

            // Sets the synthesis output format.
            // The full list of supported format can be found here:
            // https://docs.microsoft.com/azure/cognitive-services/speech-service/rest-text-to-speech#audio-outputs
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);

            // Creates a speech synthesizer using file as audio output.
            // Replace with your own audio file name.
            using (var fileOutput = AudioConfig.FromWavFileInput(FileName))
            {
                using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var speechResult = await synthesizer.SpeakTextAsync(translatedText);
                    return speechResult;
                }
            }
        }
    }
}