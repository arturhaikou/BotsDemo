using BotsDemo.Data.Contexts;
using BotsDemo.TeamsCommandBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotsDemo.TeamsCommandBot.Actions
{
    public class UpdateTodoItemActionHandler(TodoDbContext dbContext, IGetTodoItemsAdaptiveCardService service) : IAdaptiveCardActionHandler
    {
        public string TriggerVerb => "updateTodoItem";

        public AdaptiveCardResponse AdaptiveCardResponse => AdaptiveCardResponse.ReplaceForAll;

        public async Task<InvokeResponse> HandleActionInvokedAsync(ITurnContext turnContext, object cardData, CancellationToken cancellationToken = default)
        {
            (var id, var text) = GetIdAndText(cardData);
            await UpdateItemAsync(id, text, turnContext.Activity.From.Id);
            var cardContent = await service.GetTodoItemsAdaptiveCardAsync(turnContext);

            return InvokeResponseFactory.AdaptiveCard(JsonConvert.DeserializeObject(cardContent));
        }

        private async Task UpdateItemAsync(int id, string text, string userId)
        {
            var item = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            item.Text = text;
            await dbContext.SaveChangesAsync();
        }

        private (int Id, string Text) GetIdAndText(object cardData)
        {
            var jObject = JObject.Parse(cardData.ToString());
            var jProperty = jObject.First as JProperty;
            var id = int.Parse(jProperty.Name.Split('-').Last());
            var value = jProperty.Value.Value<string>();
            return (id, value);
        }
    }
}
