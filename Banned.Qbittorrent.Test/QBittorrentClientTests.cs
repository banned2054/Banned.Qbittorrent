using Banned.Qbittorrent.Exceptions;
using System.Net;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class QBittorrentClientTests
{
    [Test]
    public async Task Create_LogsInNegotiatesVersionAndInitializesServices()
    {
        var handler = new StubHttpMessageHandler(request => request.RequestUri!.AbsolutePath switch
        {
            "/api/v2/auth/login"        => Response(HttpStatusCode.OK, "Ok."),
            "/api/v2/app/webapiVersion" => Response(HttpStatusCode.OK, "2.15.1"),
            _                           => Response(HttpStatusCode.OK, "probe")
        });
        using var httpClient = new HttpClient(handler);

        var client = await QBittorrentClient.Create("http://localhost:8080", "user", "password", httpClient);

        Multiple(() =>
        {
            That(client.Application, Is.Not.Null);
            That(client.Authentication, Is.Not.Null);
            That(client.Search, Is.Not.Null);
            That(client.Torrent, Is.Not.Null);
            That(client.TorrentCreator, Is.Not.Null);
            That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                "/api/v2/auth/login",
                "/api/v2/app/webapiVersion"
            ]));
            That(Uri.UnescapeDataString(handler.Requests[0].Body!), Does.Contain("username=user"));
            That(Uri.UnescapeDataString(handler.Requests[0].Body!), Does.Contain("password=password"));
        });

        client.Dispose();
        using var response = await httpClient.GetAsync("http://localhost:8080/probe");
        That(await response.Content.ReadAsStringAsync(), Is.EqualTo("probe"));
    }

    [Test]
    public async Task ExpiredSession_UsesAuthenticationServiceToRelogAndReplayRequest()
    {
        var versionRequestCount = 0;
        var handler = new StubHttpMessageHandler(request => request.RequestUri!.AbsolutePath switch
        {
            "/api/v2/auth/login"        => Response(HttpStatusCode.OK, "Ok."),
            "/api/v2/app/webapiVersion" => Response(HttpStatusCode.OK, "2.15.1"),
            "/api/v2/app/version" when ++versionRequestCount == 1 =>
                Response(HttpStatusCode.Forbidden, "session expired"),
            "/api/v2/app/version" => Response(HttpStatusCode.OK, "5.1.2"),
            _                     => Response(HttpStatusCode.NotFound, "unexpected")
        });
        using var httpClient = new HttpClient(handler);
        using var client     = await QBittorrentClient.Create("http://localhost:8080", "user", "password", httpClient);

        var version = await client.Application.GetVersion();

        Multiple(() =>
        {
            That(version, Is.EqualTo("5.1.2"));
            That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                "/api/v2/auth/login",
                "/api/v2/app/webapiVersion",
                "/api/v2/app/version",
                "/api/v2/auth/login",
                "/api/v2/app/version"
            ]));
        });
    }

    [Test]
    public void Create_RejectsFailedLoginResponseWithoutNegotiatingVersion()
    {
        var       handler    = new StubHttpMessageHandler(_ => Response(HttpStatusCode.OK, "Fails."));
        using var httpClient = new HttpClient(handler);

        ThrowsAsync<QbittorrentLoginFailedException>(async () =>
                                                         await QBittorrentClient.Create("http://localhost:8080", "user",
                                                                  "bad-password", httpClient));

        Multiple(() =>
        {
            That(handler.Requests, Has.Count.EqualTo(1));
            That(handler.Requests[0].Uri.AbsolutePath, Is.EqualTo("/api/v2/auth/login"));
        });
    }

    private static HttpResponseMessage Response(HttpStatusCode statusCode, string body) => new(statusCode)
    {
        Content = new StringContent(body)
    };
}
