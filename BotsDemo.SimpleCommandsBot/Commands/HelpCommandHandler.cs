using Microsoft.Bot.Builder;
using Microsoft.TeamsFx.Conversation;

namespace BotsDemo.SimpleCommandsBot.Commands
{
    public class HelpCommandHandler : ITeamsCommandHandler
    {
        public IEnumerable<ITriggerPattern> TriggerPatterns => [new RegExpTrigger("help")];

        public Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            var text = @"
            Here are available commands:
            - help: to list available commands
            - getcard: to show adaptive card
            ";

            // Build the response activity  
            var activity = MessageFactory.Text(text);

            ICommandResponse commandResponse = new ActivityCommandResponse(activity);
            
            // Send response  
            return Task.FromResult(commandResponse);
        }
    }
}
