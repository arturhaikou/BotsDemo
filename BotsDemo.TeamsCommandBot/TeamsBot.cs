using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

namespace BotsDemo.TeamsCommandBot
{
    /// <summary>
    /// Bot handler.
    /// You can add your customization code here to extend your bot logic if needed.
    /// </summary>
    public class TeamsBot : TeamsActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = """Welcome to the Todo Command Bot! I can help you with a todo items list. Type "/todo" to get started.""";
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
