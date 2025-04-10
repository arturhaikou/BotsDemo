using BotsDemo.Data.Contexts;
using BotsDemo.Data.Models;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Teams.AI.AI.Action;
using System.Net.NetworkInformation;

namespace BotsDemo.ActionsAIBot.Actions
{
    public class ActionHandlers(TodoDbContext dbContext)
    {
        [Action("getTodoItems")]
        public async Task<string> GetItemsAsync([ActionTurnContext] ITurnContext context)
        {
            var items = await dbContext.TodoItems.Where(item => item.UserId == context.Activity.From.Id).ToListAsync();

            if (items.Any())
            {
                var formattedTodoItemsList = string.Join(";", items.Select(item => $"Id: {item.Id}, Text: {item.Text}"));
                var response = GetItemsResponse(formattedTodoItemsList);
                return formattedTodoItemsList;
            }

            return "The Todo Items list is empty.";
        }

        private string GetItemsResponse(string formattedTodoItemsList) => $"""
            Items:
            {formattedTodoItemsList}
            """;

        [Action("insertTodoItem")]
        public async Task<string> InsertTodoItemAsync([ActionTurnContext] ITurnContext context, [ActionParameters] Dictionary<string, object> entities)
        {
            var todoItem = new TodoItem
            {
                Text = entities["text"].ToString(),
                UserId = context.Activity.From.Id
            };

            dbContext.TodoItems.Add(todoItem);
            await dbContext.SaveChangesAsync();

            return $"""
                A new Item was added.
                Item: Id: {todoItem.Id}, Text: {todoItem.Text}
                """
            ;
        }

        [Action("updateTodoItem")]
        public async Task<string> UpdateTodoItemAsync([ActionTurnContext] ITurnContext context, [ActionParameters] Dictionary<string, object> entities)
        {
            var todoItem = await dbContext.TodoItems.FirstOrDefaultAsync(item => item.Id == int.Parse(entities["id"].ToString()) && item.UserId == context.Activity.From.Id);

            if (todoItem is not null)
            {
                todoItem.Text = entities["text"].ToString();
                await dbContext.SaveChangesAsync();
                return $"""
                    The item was updated.
                    Id: {todoItem.Id}, Text: {todoItem.Text}
                    """
                ;
            }

            return $"The item was not found with Id: {entities["id"]}";
        }

        [Action("removeTodoItem")]
        public async Task<string> RemoveTodoItemAsync([ActionTurnContext] ITurnContext context, [ActionParameters] Dictionary<string, object> entities)
        {
            var todoItem = await dbContext.TodoItems.FirstOrDefaultAsync(item => item.Id == int.Parse(entities["id"].ToString()) && item.UserId == context.Activity.From.Id);

            if (todoItem is not null)
            {
                dbContext.TodoItems.Remove(todoItem);
                await dbContext.SaveChangesAsync();
                return "Removed";
            }

            return $"The item was not found with Id: {entities["id"]}";
        }
    }
}
