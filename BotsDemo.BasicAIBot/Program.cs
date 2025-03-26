using BotsDemo.BasicAIBot;
using BotsDemo.BasicAIBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Teams.AI;
using Microsoft.Teams.AI.AI.Models;
using Microsoft.Teams.AI.AI.Planners;
using Microsoft.Teams.AI.AI.Prompts;
using Microsoft.Teams.AI.AI;
using BotsDemo.BasicAIBot.HttpClientHandlers;
using BotsDemo.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using BotsDemo.BasicAIBot.ActionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("TodoItemsDB"), ServiceLifetime.Singleton, ServiceLifetime.Singleton);
builder.Services.AddSingleton<TodoItemsActionHandler>();
// Prepare Configuration for ConfigurationBotFrameworkAuthentication
var config = builder.Configuration.Get<ConfigOptions>();
builder.Configuration["MicrosoftAppType"] = config.BOT_TYPE;
builder.Configuration["MicrosoftAppId"] = config.BOT_ID;
builder.Configuration["MicrosoftAppPassword"] = config.BOT_PASSWORD;
builder.Configuration["MicrosoftAppTenantId"] = config.BOT_TENANT_ID;
// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
builder.Services.AddSingleton(LoggerFactory.Create(configure => configure.AddConsole()));
// Create the Cloud Adapter with error handling enabled.
// Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
// register the same adapter instance for both types.
builder.Services.AddSingleton<TeamsAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<TeamsAdapter>());
builder.Services.AddSingleton<BotAdapter>(sp => sp.GetService<TeamsAdapter>());

builder.Services.AddSingleton<IStorage, MemoryStorage>();

builder.Services.AddSingleton<OpenAIModel>(sp => new(
    new OpenAIModelOptions(config.OpenAI.ApiKey, config.OpenAI.DefaultModel)
    {
        LogRequests = true
    },
    sp.GetService<ILoggerFactory>(),
    new HttpClient(new OllamaHttpClientHandler(config.OllamaBaseAddress))
));

// Create the bot as transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot>(sp =>
{
    // Create loggers
    ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

    // Create Prompt Manager
    PromptManager prompts = new(new()
    {
        PromptFolder = "./Prompts"
    });

    // Create ActionPlanner
    ActionPlanner<AppState> planner = new(
        options: new(
            model: sp.GetService<OpenAIModel>(),
            prompts: prompts,
            defaultPrompt: async (context, state, planner) =>
            {
                PromptTemplate template = prompts.GetPrompt("chat");
                return await Task.FromResult(template);
            }
        ),
        loggerFactory: loggerFactory
    );

    AIOptions<AppState> options = new(planner);
    //options.EnableFeedbackLoop = true;

    Application<AppState> app = new ApplicationBuilder<AppState>()
        .WithAIOptions(options)
        .WithStorage(sp.GetService<IStorage>())
        .Build();

    app.OnConversationUpdate("membersAdded", async (turnContext, turnState, cancellationToken) =>
    {
        var welcomeText = "How can I help you today?";
        foreach (var member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
            }
        }
    });

    app.OnFeedbackLoop((turnContext, turnState, feedbackLoopData, _) =>
    {
        Console.WriteLine($"Your feedback is {turnContext.Activity.Value.ToString()}");
        return Task.CompletedTask;
    });

    var actionHandler = sp.GetService<TodoItemsActionHandler>();
    app.AI.ImportActions(actionHandler);

    return app;
});

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