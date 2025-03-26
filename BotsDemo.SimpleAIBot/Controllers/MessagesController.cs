using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace BotsDemo.SimpleAIBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(IBot bot, IBotFrameworkHttpAdapter adapter) : ControllerBase
    {
        [HttpPost]
        public async Task PostAsync() => await adapter.ProcessAsync(Request, Response, bot);
    }
}
