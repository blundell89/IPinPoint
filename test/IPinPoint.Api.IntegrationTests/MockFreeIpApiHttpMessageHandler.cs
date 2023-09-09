using System.Collections.Concurrent;

namespace IPinPoint.Api.IntegrationTests;

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
    
    public IReadOnlyList<HttpRequestMessage> Invocations => _invocations.ToList().AsReadOnly();
}