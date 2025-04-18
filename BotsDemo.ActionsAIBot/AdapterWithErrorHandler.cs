﻿using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;

namespace BotsDemo.ActionsAIBot
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<CloudAdapter> logger)
            : base(auth, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                // NOTE: In production environment, you should consider logging this to
                // Azure Application Insights. Visit https://aka.ms/bottelemetry to see how
                // to add telemetry capture to your bot.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Only send error message for user messages, not for other message types so the bot doesn't spam a channel or chat.
                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Send a message to the user
                    await turnContext.SendActivityAsync($"The bot encountered an unhandled error: {exception.Message}");
                    await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                    // Send a trace activity
                    await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
                }
            };
        }
    }
}