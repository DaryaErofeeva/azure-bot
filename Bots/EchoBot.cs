// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        private static readonly string subscriptionKey = "a716a915e43a4d8e9f12f321927b4ef3";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string route = "/translate?api-version=3.0&to=de&to=it&to=ja&to=th";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var replyText = await TranslateText(turnContext.Activity.Text);
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
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
            return await Translator.TranslateTextRequest(subscriptionKey, endpoint, route, textToTranslate);
        }
    }
}