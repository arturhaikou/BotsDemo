using BotsDemo.Data.Contexts;
using BotsDemo.Data.Models;
using BotsDemo.TeamsCommandBot.Models;
using BotsDemo.TeamsCommandBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace BotsDemo.TeamsCommandBot.Actions
{
    public class AddTodoItemActionHandler(TodoDbContext dbContext, IGetTodoItemsAdaptiveCardService service) : IAdaptiveCardActionHandler
    {
        public string TriggerVerb => "addTodoItem";

        public AdaptiveCardResponse AdaptiveCardResponse => AdaptiveCardResponse.ReplaceForAll;

        public async Task<InvokeResponse> HandleActionInvokedAsync(ITurnContext turnContext, object cardData, CancellationToken cancellationToken = default)
        {
            var addTodoItemModel = JsonConvert.DeserializeObject<AddTodoItemModel>(cardData.ToString());
            await AddItemAsync(addTodoItemModel, turnContext.Activity.From.Id);
            var cardContent = await service.GetTodoItemsAdaptiveCardAsync(turnContext);

            return InvokeResponseFactory.AdaptiveCard(JsonConvert.DeserializeObject(cardContent));
        }

        private async Task AddItemAsync(AddTodoItemModel model, string userId)
        {
            var todoItem = new TodoItem { Text = model.InputText, UserId = userId };
            dbContext.TodoItems.Add(todoItem);
            await dbContext.SaveChangesAsync();
        }
    }
}
