using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Services;
using System.Net;
using System.Net.Http.Headers;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class NetServiceTests
{
    [Test]
    public async Task Get_EnsuresLoginBeforeSendingRequest()
    {
        var authCalls = new List<bool>();
        var (netService, httpClient, handler) = CreateNetService(_ => Ok("response"));
        using (netService)
        using (httpClient)
        {
            netService.EnsureLoggedInAsyncHandler = (force, _) =>
            {
                authCalls.Add(force);
                return Task.CompletedTask;
            };

            var response = await netService.Get("api/v2/app/version");

            Multiple(() =>
            {
                That(response, Is.EqualTo("response"));
                That(authCalls, Is.EqualTo([false]));
                That(handler.Requests, Has.Count.EqualTo(1));
            });
        }
    }

    [Test]
    public async Task Get_ForceRelogsAndReplaysOnceAfterUnauthorizedResponse()
    {
        var authCalls      = new List<bool>();
        var responseNumber = 0;
        var (netService, httpClient, handler) = CreateNetService(_ =>
        {
            responseNumber++;
            return responseNumber == 1
                ? Response(HttpStatusCode.Unauthorized, "expired")
                : Ok("recovered");
        });
        using (netService)
        using (httpClient)
        {
            netService.EnsureLoggedInAsyncHandler = (force, _) =>
            {
                authCalls.Add(force);
                return Task.CompletedTask;
            };

            var response = await netService.Get("api/v2/app/version", maxRetries : 1);

            Multiple(() =>
            {
                That(response, Is.EqualTo("recovered"));
                That(authCalls, Is.EqualTo([false, true]));
                That(handler.Requests, Has.Count.EqualTo(2));
            });
        }
    }

    [Test]
    public void AuthenticationEndpoint_SkipsLoginCheckAndAuthRetry()
    {
        var authCallCount = 0;
        var (netService, httpClient, handler) =
            CreateNetService(_ => Response(HttpStatusCode.Unauthorized, "bad credentials"));
        using (netService)
        using (httpClient)
        {
            netService.EnsureLoggedInAsyncHandler = (_, _) =>
            {
                authCallCount++;
                return Task.CompletedTask;
            };

            ThrowsAsync<QbittorrentUnauthorizedException>(async () =>
                                                              await netService.Post("api/v2/auth/login",
                                                                       maxRetries : 1));

            Multiple(() =>
            {
                That(authCallCount, Is.Zero);
                That(handler.Requests, Has.Count.EqualTo(1));
            });
        }
    }

    [TestCase(HttpStatusCode.BadRequest, typeof(QbittorrentBadRequestException))]
    [TestCase(HttpStatusCode.Unauthorized, typeof(QbittorrentUnauthorizedException))]
    [TestCase(HttpStatusCode.Forbidden, typeof(QbittorrentForbiddenException))]
    [TestCase(HttpStatusCode.NotFound, typeof(QbittorrentNotFoundException))]
    [TestCase(HttpStatusCode.Conflict, typeof(QbittorrentConflictException))]
    [TestCase(HttpStatusCode.InternalServerError, typeof(QbittorrentServerErrorException))]
    public void ErrorResponse_MapsToSpecificException(HttpStatusCode statusCode, Type expectedExceptionType)
    {
        var (netService, httpClient, _) = CreateNetService(_ => Response(statusCode, "failure"));
        using (netService)
        using (httpClient)
        {
            var exception = ThrowsAsync(expectedExceptionType, async () =>
                                            await netService.Get("api/v2/test", maxRetries : 1));

            That(exception, Is.TypeOf(expectedExceptionType));
        }
    }

    [Test]
    public void UnknownErrorResponse_PreservesStatusAndBody()
    {
        const int unknownStatusCode = 418;
        var (netService, httpClient, _) =
            CreateNetService(_ => Response((HttpStatusCode)unknownStatusCode, "brew failed"));
        using (netService)
        using (httpClient)
        {
            var exception =
                ThrowsAsync<QbittorrentException>(async () => await netService.Get("api/v2/test", maxRetries : 1));

            Multiple(() =>
            {
                That(exception!.StatusCode, Is.EqualTo(unknownStatusCode));
                That(exception.Message, Does.Contain("brew failed"));
            });
        }
    }

    [Test]
    public async Task RetryableResponse_RetriesAndReturnsSuccessfulBody()
    {
        var responseNumber = 0;
        var (netService, httpClient, handler) = CreateNetService(_ =>
        {
            responseNumber++;
            if (responseNumber != 1) return Ok("recovered");

            var response = Response(HttpStatusCode.ServiceUnavailable, "busy");
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.Zero);
            return response;
        });
        using (netService)
        using (httpClient)
        {
            var result = await netService.Get("api/v2/test", maxRetries : 2);

            Multiple(() =>
            {
                That(result, Is.EqualTo("recovered"));
                That(handler.Requests, Has.Count.EqualTo(2));
            });
        }
    }

    [Test]
    public void PostNetworkFailure_IsNotRetriedBecauseRequestMayHaveSideEffects()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => throw new HttpRequestException("connection failed"));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentServerErrorException>(async () =>
                                                             await netService.Post("api/v2/torrents/delete",
                                                                      maxRetries : 3));

            That(handler.Requests, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void CanceledRequest_StopsBeforeSendingHttpRequest()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var (netService, httpClient, handler) = CreateNetService(_ => Ok("unexpected"));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<OperationCanceledException>(async () =>
                                                        await netService.Get("api/v2/test", ct : cancellation.Token));

            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public void PostWithFiles_MissingFileFailsBeforeSendingHttpRequest()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.torrent");
        var (netService, httpClient, handler) = CreateNetService(_ => Ok("unexpected"));
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentFileNotFoundException>(async () =>
                                                              await netService.PostWithFiles("api/v2/torrents/add",
                                                                       null, [missingPath]));

            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task PostWithFiles_SendsMultipartFieldsAndFileContent()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.torrent");
        await File.WriteAllTextAsync(filePath, "torrent-payload");
        try
        {
            var (netService, httpClient, handler) = CreateNetService(_ => Ok("Ok."));
            using (netService)
            using (httpClient)
            {
                await netService.PostWithFiles("api/v2/torrents/add",
                                               new Dictionary<string, string> { ["category"] = "linux" }, [filePath]);

                Multiple(() =>
                {
                    That(handler.Requests, Has.Count.EqualTo(1));
                    That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
                    That(handler.Requests[0].Body, Does.Contain("name=category"));
                    That(handler.Requests[0].Body, Does.Contain("linux"));
                    That(handler.Requests[0].Body, Does.Contain(Path.GetFileName(filePath)));
                    That(handler.Requests[0].Body, Does.Contain("torrent-payload"));
                });
            }
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task Dispose_DoesNotDisposeCallerOwnedHttpClient()
    {
        var       handler    = new StubHttpMessageHandler(_ => Ok("still available"));
        using var httpClient = new HttpClient(handler);
        var       netService = new NetService("http://localhost:8080", httpClient);

        netService.Dispose();
        using var response = await httpClient.GetAsync("http://localhost:8080/probe");

        That(await response.Content.ReadAsStringAsync(), Is.EqualTo("still available"));
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var handler    = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_15_1);
        return (netService, httpClient, handler);
    }

    private static HttpResponseMessage Ok(string body) => Response(HttpStatusCode.OK, body);

    private static HttpResponseMessage Response(HttpStatusCode statusCode, string body) => new(statusCode)
    {
        Content = new StringContent(body)
    };
}
