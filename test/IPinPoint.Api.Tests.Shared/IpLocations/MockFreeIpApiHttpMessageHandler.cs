using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;

namespace IPinPoint.Api.Tests.Shared.IpLocations;

public class MockFreeIpApiHttpMessageHandler : HttpMessageHandler
{
    private readonly ConcurrentBag<HttpRequestMessage> _invocations = new();
    private readonly List<(Predicate<HttpRequestMessage> shouldHandle, Func<HttpRequestMessage, HttpResponseMessage> handle)> _handlers = new();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (_, handler) = _handlers.FirstOrDefault(x => x.shouldHandle(request));
        
        if (handler is null)
            throw new InvalidOperationException("No handler configured for request");
        
        _invocations.Add(request);
        return Task.FromResult(handler(request));
    }

    public void AddResponse(Predicate<HttpRequestMessage> shouldHandle,
        Func<HttpRequestMessage, HttpResponseMessage> handle)
    {
        _handlers.Add((shouldHandle, handle));
    }
    
    public void AddSuccessResponse(Predicate<HttpRequestMessage> shouldHandle,
        JsonDocument body)
    {
        _handlers.Add((shouldHandle, handle: _ =>
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Content = new StringContent(body.RootElement.ToString()!);
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            return responseMessage;
        }));
    }
    
    public IReadOnlyList<HttpRequestMessage> Invocations => _invocations.ToList().AsReadOnly();
}