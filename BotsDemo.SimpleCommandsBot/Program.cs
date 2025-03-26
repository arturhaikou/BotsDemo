using BotsDemo.SimpleCommandsBot.Adapters;
using BotsDemo.SimpleCommandsBot.Bots;
using BotsDemo.SimpleCommandsBot.Commands;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Options;
using Microsoft.TeamsFx.Conversation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBot, SimpleCommandsBot>();
var app = builder.Build();

RegisterCommands(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

void RegisterCommands(IServiceProvider serviceProvider)
{
    IList<ITeamsCommandHandler> commands = [new HelpCommandHandler(), new GetCardCommandHandler()];
    var adapter = serviceProvider.GetRequiredService<IBotFrameworkHttpAdapter>() as AdapterWithErrorHandler;
    adapter.Use(new CommandResponseMiddleware(commands));
}
