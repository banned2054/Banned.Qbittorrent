using System.Net;
using Banned.Qbittorrent.Services;

namespace Banned.Qbittorrent.Test;

public class NetServiceUnitTest
{
    [Test]
    public async Task TestApiVersionReauthenticatesWhenForbidden()
    {
        using var handler = new ReauthenticationHandler();
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        using var _ = new AuthenticationService(netService, "admin", "adminadmin");
        var applicationService = new ApplicationService(netService);

        var response = await applicationService.GetApiVersion();

        Assert.That(response.ToString(), Is.EqualTo("2.9.3"));
        Assert.That(handler.LoginRequests, Is.EqualTo(1));
        Assert.That(handler.WebApiVersionRequests, Is.EqualTo(2));
    }

    [Test]
    public async Task TestReauthenticatesAndRetriesAfterForbidden()
    {
        using var handler = new ReauthenticationHandler();
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        using var _ = new AuthenticationService(netService, "admin", "adminadmin");

        var response = await netService.Get("api/v2/app/version");

        Assert.That(response, Is.EqualTo("v4.6.4"));
        Assert.That(handler.LoginRequests, Is.EqualTo(1));
        Assert.That(handler.VersionRequests, Is.EqualTo(2));
    }

    private sealed class ReauthenticationHandler : HttpMessageHandler
    {
        public int LoginRequests { get; private set; }

        public int VersionRequests { get; private set; }

        public int WebApiVersionRequests { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            if (request.RequestUri?.AbsolutePath.EndsWith("/api/v2/auth/login") == true)
            {
                LoginRequests++;
                return Task.FromResult(CreateResponse(HttpStatusCode.OK, string.Empty));
            }

            if (request.RequestUri?.AbsolutePath.EndsWith("/api/v2/app/webapiVersion") == true)
            {
                WebApiVersionRequests++;
                return Task.FromResult(WebApiVersionRequests == 1
                                           ? CreateResponse(HttpStatusCode.Forbidden, "Forbidden")
                                           : CreateResponse(HttpStatusCode.OK, "2.9.3"));
            }

            if (request.RequestUri?.AbsolutePath.EndsWith("/api/v2/app/version") == true)
            {
                VersionRequests++;
                return Task.FromResult(VersionRequests == 1
                                           ? CreateResponse(HttpStatusCode.Forbidden, "Forbidden")
                                           : CreateResponse(HttpStatusCode.OK, "v4.6.4"));
            }

            return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "Not Found"));
        }

        private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string content)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
        }
    }
}
