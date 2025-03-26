using AdaptiveCards.Templating;
using BotsDemo.Data.Contexts;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;

namespace BotsDemo.TeamsCommandBot.Services
{
    public class GetTodoItemsAdaptiveCardService(TodoDbContext dbContext) : IGetTodoItemsAdaptiveCardService
    {
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "GetTotoItemsAdaptiveCard.json");
        public async Task<string> GetTodoItemsAdaptiveCardAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var items = await dbContext.TodoItems.ToListAsync();
            var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);
            
            return new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new
                {
                    Title = "Todo Items List",
                    Items = items
                }
            );
        }
    }
}
