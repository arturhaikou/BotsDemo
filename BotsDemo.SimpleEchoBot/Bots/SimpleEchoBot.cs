using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace BotsDemo.SimpleEchoBot.Bots
{
    public class SimpleEchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyActivity = MessageFactory.Text($"You said {turnContext.Activity.Text}.");
            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }
    }
}
