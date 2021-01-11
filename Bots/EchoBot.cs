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
        private const string ContentType = "audio/mpeg";
        private const string WelcomeMessage = "Bonjour et bienvenue!";
        private const string StartCommand = "/start";
        private const string AudioNamePrefix = "Au message:";

        private const string InstructionMessage =
            "Commençons!\n Entrez la phrase dans n’importe quelle langue et je vais la traduire pour vous en Français.";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Equals(StartCommand))
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(InstructionMessage, InstructionMessage), cancellationToken);
            }
            else
            {
                try
                {
                    var translatedText = await TranslateText(turnContext.Activity.Text);
                    foreach (var activity in await GetResultActivities(translatedText))
                    {
                        await turnContext.SendActivityAsync(activity, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(e.StackTrace, e.Message), cancellationToken);
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

        private static async Task<string> TranslateText(string textToTranslate)
        {
            return await Translator.TranslateTextRequest(textToTranslate);
        }

        private static async Task<List<IMessageActivity>> GetResultActivities(string translatedText)
        {
            var messagesList = new List<IMessageActivity>();
            messagesList.Add(MessageFactory.Text(translatedText, translatedText));

            var speechResult = await Speech.Speech.SynthesizeAudioAsync(translatedText);
            if (speechResult.Reason != ResultReason.SynthesizingAudioCompleted) return messagesList;
            var name = translatedText;
            if (translatedText.Length > 7)
            {
                name = $"{translatedText.Substring(0, 7)}...";
            }

            messagesList.Add(MessageFactory.Attachment(GetLocalFileAttachment(speechResult.AudioData, name)));

            return messagesList;
        }

        private static Attachment GetLocalFileAttachment(Object audio, String name)
        {
            var attachment = new Attachment
            {
                ContentType = ContentType,
                Content = audio,
                Name = $"{AudioNamePrefix} {name}"
            };
            return attachment;
        }
    }
}