// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Translate;
using Microsoft.CognitiveServices.Speech;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        private static readonly string ContentType = "audio/mpeg";
        private static readonly string WelcomeMessage = "Bonjour et bienvenue!";

        private static readonly string InstructionMessage =
            "Commençons! Entrez la phrase dans n’importe quelle langue et je vais la traduire pour vous en Français.";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Equals("/start"))
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(InstructionMessage, InstructionMessage), cancellationToken);
            }
            else
            {
                var translatedText = await TranslateText(turnContext.Activity.Text);
                foreach (var activity in await GetResultActivities(translatedText))
                {
                    await turnContext.SendActivityAsync(activity, cancellationToken);
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage, WelcomeMessage),
                        cancellationToken);
                }
            }
        }

        private async Task<string> TranslateText(string textToTranslate)
        {
            return await Translator.TranslateTextRequest(textToTranslate);
        }

        private async Task<List<IMessageActivity>> GetResultActivities(string translatedText)
        {
            var messagesList = new List<IMessageActivity>();
            messagesList.Add(MessageFactory.Text(translatedText, translatedText));

            var speechResult = await Speech.Speech.SynthesizeAudioAsync(translatedText);
            if (speechResult.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                var name = translatedText;
                if (translatedText.Length > 7)
                {
                    name = String.Format("{0}...", translatedText.Substring(0, 7));
                }

                messagesList.Add(MessageFactory.Attachment(GetLocalFileAttachment(speechResult.AudioData, name)));
            }

            return messagesList;
        }

        private Attachment GetLocalFileAttachment(Object audio, String name)
        {
            Attachment attachment = new Attachment
            {
                ContentType = ContentType,
                Content = audio,
                Name = name
            };
            return attachment;
        }
    }
}