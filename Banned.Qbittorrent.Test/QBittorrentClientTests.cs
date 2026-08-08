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
            "/api/v2/app/version"       => Response(HttpStatusCode.OK, "5.1.2"),
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
            That(client.QbittorrentVersion, Is.EqualTo("5.1.2"));
            That(handler.Requests.Select(request => request.Uri.AbsolutePath), Is.EqualTo([
                "/api/v2/auth/login",
                "/api/v2/app/webapiVersion",
                "/api/v2/app/version"
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
            "/api/v2/app/version" when ++versionRequestCount == 2 =>
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

    [TestCase("v4.3.2", false)]
    [TestCase("v4.3.3", true)]
    public async Task RenameTorrentFile_UsesApplicationVersionSpecificParameters(string applicationVersion,
                                                                                 bool   pathBased)
    {
        var handler = CreateVersionedHandler(applicationVersion);
        using var httpClient = new HttpClient(handler);
        using var client     = await QBittorrentClient.Create("http://localhost:8080", "user", "password", httpClient);

        await client.Torrent.RenameTorrentFile("abc", "old.txt", "new.txt");

        var renameRequest = handler.Requests.Single(request => request.Uri.AbsolutePath == "/api/v2/torrents/renameFile");
        var form          = Uri.UnescapeDataString(renameRequest.Body!);
        Multiple(() =>
        {
            That(client.QbittorrentVersion, Is.EqualTo(applicationVersion));
            That(form, pathBased ? Does.Contain("oldPath=old.txt") : Does.Contain("id=0"));
            That(form, pathBased ? Does.Contain("newPath=new.txt") : Does.Contain("name=new.txt"));
            That(form, pathBased ? Does.Not.Contain("id=") : Does.Not.Contain("oldPath="));
        });
    }

    [TestCase("v4.3.2", false)]
    [TestCase("v4.3.3", true)]
    public async Task RenameTorrentFolder_RequiresQbittorrent433(string applicationVersion, bool supported)
    {
        var handler = CreateVersionedHandler(applicationVersion);
        using var httpClient = new HttpClient(handler);
        using var client     = await QBittorrentClient.Create("http://localhost:8080", "user", "password", httpClient);
        var requestCountBeforeRename = handler.Requests.Count;

        if (supported)
            await client.Torrent.RenameTorrentFolder("abc", "old", "new");
        else
            ThrowsAsync<NotSupportedException>(async () =>
                await client.Torrent.RenameTorrentFolder("abc", "old", "new"));

        That(handler.Requests.Count, Is.EqualTo(requestCountBeforeRename + (supported ? 1 : 0)));
    }

    private static StubHttpMessageHandler CreateVersionedHandler(string applicationVersion) =>
        new(request => request.RequestUri!.AbsolutePath switch
        {
            "/api/v2/auth/login"        => Response(HttpStatusCode.OK, "Ok."),
            "/api/v2/app/webapiVersion" => Response(HttpStatusCode.OK, "2.7.0"),
            "/api/v2/app/version"       => Response(HttpStatusCode.OK, applicationVersion),
            "/api/v2/torrents/files"   => Response(HttpStatusCode.OK, "[{\"index\":0,\"name\":\"old.txt\"}]"),
            _                           => Response(HttpStatusCode.OK, "")
        });

    private static HttpResponseMessage Response(HttpStatusCode statusCode, string body) => new(statusCode)
    {
        Content = new StringContent(body)
    };
}
