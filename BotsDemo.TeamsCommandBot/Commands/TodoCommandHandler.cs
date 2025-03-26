using BotsDemo.Data.Contexts;
using BotsDemo.TeamsCommandBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace BotsDemo.TeamsCommandBot.Commands
{
    public class TodoCommandHandler(TodoDbContext dbContext, IGetTodoItemsAdaptiveCardService service) : ITeamsCommandHandler
    {
        public IEnumerable<ITriggerPattern> TriggerPatterns => [new StringTrigger("/todo")];

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            var cardContent = await service.GetTodoItemsAdaptiveCardAsync(turnContext);

            var activity = MessageFactory.Attachment
            (
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                }
            );

            return new ActivityCommandResponse(activity);
        }
    }
}
