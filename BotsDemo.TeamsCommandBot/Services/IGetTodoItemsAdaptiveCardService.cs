using Microsoft.Bot.Builder;

namespace BotsDemo.TeamsCommandBot.Services
{
    public interface IGetTodoItemsAdaptiveCardService
    {
        Task<string> GetTodoItemsAdaptiveCardAsync(ITurnContext turnContext, CancellationToken cancellationToken = default);
    }
}
