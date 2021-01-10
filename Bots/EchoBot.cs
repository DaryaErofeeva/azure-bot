// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Translate;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var translatedText = await TranslateText(turnContext.Activity.Text);
            var reply = await GetResultActivity(translatedText);
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Bonjour et bienvenue!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText),
                        cancellationToken);
                }
            }
        }

        private async Task<string> TranslateText(string textToTranslate)
        {
            return await Translator.TranslateTextRequest(textToTranslate);
        }

        private async Task<Activity> GetResultActivity(string translatedText)
        {
            var resultAudio = await Speech.Speech.SynthesizeAudioAsync(translatedText);
            if (resultAudio != null)
            {
                MessageFactory.Attachment(GetLocalFileAttachment(resultAudio, translatedText));
            }

            return MessageFactory.Text(translatedText, translatedText);
        }

        private Attachment GetLocalFileAttachment(Object audio, string text)
        {
            Attachment attachment = new Attachment
            {
                ContentType = "audio/mpeg",
                Content = audio,
                Name = text
            };
            return attachment;
        }
    }
}