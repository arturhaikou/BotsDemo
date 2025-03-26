using AdaptiveCards.Templating;
using BotsDemo.SimpleCommandsBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace BotsDemo.SimpleCommandsBot.Commands
{
    public class GetCardCommandHandler : ITeamsCommandHandler
    {
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "AdaptiveCards", "GetCardAdaptiveCard.json");
        public IEnumerable<ITriggerPattern> TriggerPatterns => [new RegExpTrigger("getcard")];

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

            // Render adaptive card content
            var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new GetCardModel
                {
                    Title = "Your Simple Commands Bot App is Running",
                    Body = "Congratulations! Your Simple Commands Bot App is running. Open the documentation below to learn more about how to build applications with the Teams Toolkit.",
                }
            );

            // Build attachment
            var activity = MessageFactory.Attachment
            (
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                }
            );

            // send response
            return new ActivityCommandResponse(activity);
        }
    }
}
