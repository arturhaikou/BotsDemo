using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.AI;

namespace BotsDemo.SimpleAIBot.Bots
{
    public class SimpleAIBot(IChatClient client) : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var response = await client.GetResponseAsync(new ChatMessage(ChatRole.User, turnContext.Activity.Text));
            var replyActivity = MessageFactory.Text(response.Text);
            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Welcome to the Simple AI Bot! I can help you with any questions. What would you like to know";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
                }
            }
        }
    }
}
