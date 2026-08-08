using System.Net;

namespace Banned.Qbittorrent.Test;

internal sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    : HttpMessageHandler
{
    public List<HttpRequestSnapshot> Requests { get; } = [];

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken  cancellationToken)
    {
        var body = request.Content == null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        Requests.Add(new HttpRequestSnapshot(request.Method, request.RequestUri!, body, cancellationToken));
        return responseFactory(request);
    }

    public static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
    };

    public static HttpResponseMessage BytesResponse(byte[] bytes) => new(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(bytes)
    };
}

internal sealed record HttpRequestSnapshot(
    HttpMethod        Method,
    Uri               Uri,
    string?           Body,
    CancellationToken CancellationToken);
