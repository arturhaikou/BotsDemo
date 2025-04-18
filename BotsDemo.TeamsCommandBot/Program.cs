using BotsDemo.TeamsCommandBot;
using BotsDemo.TeamsCommandBot.Commands;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.TeamsFx.Conversation;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using BotsDemo.TeamsCommandBot.Actions;
using BotsDemo.TeamsCommandBot.Services;
using BotsDemo.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("TodoItemsDB"), ServiceLifetime.Singleton, ServiceLifetime.Singleton);
builder.Services.AddSingleton<IGetTodoItemsAdaptiveCardService, GetTodoItemsAdaptiveCardService>();
// Prepare Configuration for ConfigurationBotFrameworkAuthentication
var config = builder.Configuration.Get<ConfigOptions>();
builder.Configuration["MicrosoftAppType"] = config.BOT_TYPE;
builder.Configuration["MicrosoftAppId"] = config.BOT_ID;
builder.Configuration["MicrosoftAppPassword"] = config.BOT_PASSWORD;
builder.Configuration["MicrosoftAppTenantId"] = config.BOT_TENANT_ID;
// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

// Create the Cloud Adapter with error handling enabled.
// Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
// register the same adapter instance for both types.
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
builder.Services.AddSingleton<BotAdapter>(sp => sp.GetService<CloudAdapter>());

// Create command handlers and the Conversation with command-response feature enabled.
builder.Services.AddSingleton<TodoCommandHandler>();
builder.Services.AddSingleton<AddTodoItemActionHandler>();
builder.Services.AddSingleton<RemoveTodoItemActionHandler>();
builder.Services.AddSingleton<UpdateTodoItemActionHandler>();
builder.Services.AddSingleton(sp =>
{
    var options = new ConversationOptions()
    {
        Adapter = sp.GetService<CloudAdapter>(),
        Command = new CommandOptions()
        {
            Commands = [sp.GetService<TodoCommandHandler>()]
        },
        CardAction = new CardActionOptions
        {
            Actions = [sp.GetService<AddTodoItemActionHandler>(), sp.GetService<RemoveTodoItemActionHandler>(), sp.GetService<UpdateTodoItemActionHandler>()]
        }
    };

    return new ConversationBot(options);
});

// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, TeamsBot>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();