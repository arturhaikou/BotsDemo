using BotsDemo.Data.Contexts;
using BotsDemo.TeamsCommandBot.Models;
using BotsDemo.TeamsCommandBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace BotsDemo.TeamsCommandBot.Actions
{
    public class RemoveTodoItemActionHandler(TodoDbContext dbContext, IGetTodoItemsAdaptiveCardService service) : IAdaptiveCardActionHandler
    {
        public string TriggerVerb => "removeTodoItem";

        public AdaptiveCardResponse AdaptiveCardResponse => AdaptiveCardResponse.ReplaceForAll;

        public async Task<InvokeResponse> HandleActionInvokedAsync(ITurnContext turnContext, object cardData, CancellationToken cancellationToken = default)
        {
            var removeItemModel = JsonConvert.DeserializeObject<RemoveTodoItemModel>(cardData.ToString());
            await RemoveItemAsync(removeItemModel, turnContext.Activity.From.Id);
            var cardContent = await service.GetTodoItemsAdaptiveCardAsync(turnContext);

            return InvokeResponseFactory.AdaptiveCard(JsonConvert.DeserializeObject(cardContent));
        }

        private async Task RemoveItemAsync(RemoveTodoItemModel model, string userId)
        {
            var item = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);
            dbContext.TodoItems.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }
}
