using System;

namespace BotsDemo.BasicAIBot.HttpClientHandlers
{
    public class OllamaHttpClientHandler(string baseAddress) : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && request.RequestUri.Host == "api.openai.com")
            {
                request.RequestUri = new Uri($"{baseAddress}{request.RequestUri.PathAndQuery}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
