using BotsDemo.SimpleAIBot.Options;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.AI;
using BotsDemo.SimpleAIBot.Bots;

var builder = WebApplication.CreateBuilder(args);

var ollamaOptions = new OllamaConfigOptions();

builder.Configuration.GetSection("Ollama").Bind(ollamaOptions);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddChatClient(new OllamaChatClient(new Uri(ollamaOptions.Endpoint), ollamaOptions.Model));
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, CloudAdapter>();
builder.Services.AddTransient<IBot, SimpleAIBot>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
