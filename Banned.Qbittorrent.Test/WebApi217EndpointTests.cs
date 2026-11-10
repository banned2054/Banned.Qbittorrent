using System.Net;
using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Models.Search;
using Banned.Qbittorrent.Services;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

/// <summary>
/// 覆盖 Web API v2.17.x 变更的契约测试。<br/>
/// Contract tests for the changes introduced by Web API v2.17.x.
/// </summary>
[TestFixture]
public class WebApi217EndpointTests
{
    private static readonly ApiVersion Supported  = new(2, 17);
    private static readonly ApiVersion Previous  = new(2, 16, 1);
    private static readonly ApiVersion Introduced = new(2, 16, 2);

    [Test]
    public async Task CancelTorrentMetadata_PostsSourceToCancelEndpoint()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Supported).CancelTorrentMetadata("magnet:?xt=urn:btih:abc");

            var request = handler.Requests.Single();
            Multiple(() =>
            {
                That(request.Method, Is.EqualTo(HttpMethod.Post));
                That(request.Uri!.AbsolutePath, Is.EqualTo("/api/v2/torrents/cancelFetchMetadata"));
                That(DecodeForm(request), Is.EqualTo("source=magnet:?xt=urn:btih:abc"));
            });
        }
    }

    [Test]
    public void CancelTorrentMetadata_RejectsEmptySourceBeforeSendingRequest()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (netService)
        using (httpClient)
        {
            var torrent = new TorrentService(netService, Supported);
            ThrowsAsync<ArgumentException>(async () => await torrent.CancelTorrentMetadata(" "));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public void CancelTorrentMetadata_RejectsWebApiBefore2170BeforeSendingRequest()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""), Previous);
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentNotSupportedException>(async () =>
                await new TorrentService(netService, Previous).CancelTorrentMetadata("abc123"));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public void CancelTorrentMetadata_MapsNotFoundToQbittorrentNotFoundException()
    {
        var (netService, httpClient, handler) =
            CreateNetService(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
                                 { Content = new StringContent("") });
        using (netService)
        using (httpClient)
        {
            var exception = ThrowsAsync<QbittorrentNotFoundException>(async () =>
                await new TorrentService(netService, Supported).CancelTorrentMetadata("abc123"));
            That(exception!.Message, Is.EqualTo("No matching fetchMetadata task for the given source."));
        }
    }

    [Test]
    public async Task CreateCategory_SendsShareLimitParameters()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Introduced);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Introduced).CreateCategory("movies", "/downloads/movies",
                                                                           ratioLimit : 1.5f,
                                                                           seedingTimeLimit : 120,
                                                                           inactiveSeedingTimeLimit : 60,
                                                                           shareLimitAction :
                                                                           EnumTorrentShareLimitAction.Stop,
                                                                           shareLimitsMode :
                                                                           EnumTorrentShareLimitsMode.MatchAll);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Does.Contain("category=movies"));
                That(form, Does.Contain("savePath=/downloads/movies"));
                That(form, Does.Contain("ratioLimit=1.5"));
                That(form, Does.Contain("seedingTimeLimit=120"));
                That(form, Does.Contain("inactiveSeedingTimeLimit=60"));
                That(form, Does.Contain("shareLimitAction=Stop"));
                That(form, Does.Contain("shareLimitsMode=MatchAll"));
            });
        }
    }

    [Test]
    public async Task CreateCategory_KeepsOldBehaviorWithoutShareLimitsOnPreviousWebApi()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Previous).CreateCategory("movies", "/downloads/movies");

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(DecodeForm(handler.Requests.Single()),
                     Is.EqualTo("category=movies&savePath=/downloads/movies"));
            });
        }
    }

    [Test]
    public async Task CreateCategory_RejectsShareLimitsBeforeWebApi2162ButAllows2162()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentNotSupportedException>(async () =>
                await new TorrentService(netService, Previous).CreateCategory("movies", "/downloads/movies",
                                                                              ratioLimit : 1.0f));
            That(handler.Requests, Is.Empty);
        }

        var (introducedNetService, introducedHttpClient, introducedHandler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""), Introduced);
        using (introducedNetService)
        using (introducedHttpClient)
        {
            await new TorrentService(introducedNetService, Introduced).CreateCategory("movies", "/downloads/movies",
                                                                                     ratioLimit : 1.0f);
            That(DecodeForm(introducedHandler.Requests.Single()), Does.Contain("ratioLimit=1"));
        }
    }

    [TestCase(nameof(Banned.Qbittorrent.Services.TorrentService.CreateCategory))]
    [TestCase(nameof(Banned.Qbittorrent.Services.TorrentService.EditCategory))]
    public void CategoryShareLimits_RejectValuesBelowMinusTwo(string method)
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Supported);
        using (netService)
        using (httpClient)
        {
            var torrent = new TorrentService(netService, Supported);
            var isCreate = method == nameof(Banned.Qbittorrent.Services.TorrentService.CreateCategory);
            Func<Task> seeding = isCreate
                ? async () => await torrent.CreateCategory("movies", "/downloads/movies", seedingTimeLimit : -3)
                : async () => await torrent.EditCategory("movies", seedingTimeLimit : -3);
            Func<Task> inactive = isCreate
                ? async () => await torrent.CreateCategory("movies", "/downloads/movies",
                                                           inactiveSeedingTimeLimit : -3)
                : async () => await torrent.EditCategory("movies", inactiveSeedingTimeLimit : -3);

            Multiple(() =>
            {
                ThrowsAsync<ArgumentException>(seeding);
                ThrowsAsync<ArgumentException>(inactive);
            });
            That(handler.Requests, Is.Empty);
        }
    }

    [TestCase(-2)]
    [TestCase(-1)]
    public async Task CategoryShareLimits_SendsSupportedSpecialTimeLimits(int limit)
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Introduced);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Introduced).CreateCategory("movies", "/downloads/movies",
                                                                            seedingTimeLimit : limit,
                                                                            inactiveSeedingTimeLimit : limit);

            That(DecodeForm(handler.Requests.Single()), Does.Contain($"seedingTimeLimit={limit}"));
            That(DecodeForm(handler.Requests.Single()), Does.Contain($"inactiveSeedingTimeLimit={limit}"));
        }
    }

    [Test]
    public async Task CategoryShareLimitAction_CanRestoreDefault()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Introduced);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Introduced).EditCategory("movies",
                                                                          shareLimitAction :
                                                                          EnumTorrentShareLimitAction.Default);

            That(DecodeForm(handler.Requests.Single()), Is.EqualTo("category=movies&shareLimitAction=Default"));
        }
    }

    [Test]
    public async Task EditCategory_OmitsSavePathWhenOnlyShareLimitsChange()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Introduced);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Introduced).EditCategory("movies",
                                                                         seedingTimeLimit : 90,
                                                                         shareLimitsMode :
                                                                         EnumTorrentShareLimitsMode.MatchAny);

            var form = DecodeForm(handler.Requests.Single());
            Multiple(() =>
            {
                That(form, Is.EqualTo("category=movies&seedingTimeLimit=90&shareLimitsMode=MatchAny"));
            });
        }
    }

    [Test]
    public async Task EditCategory_StillSendsSavePathWhenProvided()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Previous).EditCategory("movies", "/downloads/movies");

            Multiple(() =>
            {
                That(handler.Requests, Has.Count.EqualTo(1));
                That(DecodeForm(handler.Requests.Single()),
                     Is.EqualTo("category=movies&savePath=/downloads/movies"));
            });
        }
    }

    [Test]
    public void EditCategory_RejectsOmittedSavePathBeforeWebApi2162BeforeSendingRequest()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            ThrowsAsync<QbittorrentNotSupportedException>(async () =>
                await new TorrentService(netService, Previous).EditCategory("movies"));
            ThrowsAsync<QbittorrentNotSupportedException>(async () =>
                await new TorrentService(netService, Previous).EditCategory("movies", null));
            That(handler.Requests, Is.Empty);
        }
    }

    [Test]
    public async Task EditCategory_OmittedSavePathWorksAt2162()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Introduced);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Introduced).EditCategory("movies");
            That(DecodeForm(handler.Requests.Single()), Is.EqualTo("category=movies"));
        }
    }

    [Test]
    public async Task EditCategory_CanClearSavePathWithEmptyStringOnOlderServer()
    {
        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            await new TorrentService(netService, Previous).EditCategory("movies", "");
            That(DecodeForm(handler.Requests.Single()), Is.EqualTo("category=movies&savePath="));
        }
    }

    [Test]
    public async Task GetTorrentInfos_DeserializesErrorField()
    {
        const string json = """
            [
              {
                "hash": "abc123",
                "name": "ubuntu",
                "error": "Tracker returned DNS failure",
                "futureField": "preserved"
              }
            ]
            """;
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var infos = await new TorrentService(netService, Supported).GetTorrentInfos(["abc123"]);

            Multiple(() =>
            {
                That(infos, Has.Count.EqualTo(1));
                That(infos[0].Error, Is.EqualTo("Tracker returned DNS failure"));
            });
        }
    }

    [Test]
    public async Task GetSearchResults_DeserializesEngineFullName()
    {
        const string json = """
            {
              "results": [
                {
                  "fileName": "ubuntu.iso",
                  "engineName": "tpb",
                  "engineFullName": "The Pirate Bay",
                  "fileSize": 1024,
                  "nbSeeders": 3,
                  "nbLeechers": 1,
                  "fileUrl": "magnet:?xt=urn:btih:abc",
                  "siteUrl": "https://example.org",
                  "futureField": "preserved"
                }
              ],
              "status": "Stopped",
              "total": 1
            }
            """;
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var result = await new SearchService(netService).GetSearchResults(1);

            Multiple(() =>
            {
                That(result!.Results, Is.Not.Null);
                That(result.Results![0].EngineName, Is.EqualTo("tpb"));
                That(result.Results[0].EngineFullName, Is.EqualTo("The Pirate Bay"));
                That(result.Status!.Status, Is.EqualTo("Stopped"));
                That(result.Status.Id, Is.EqualTo(0));
            });
        }
    }

    [TestCase("null")]
    [TestCase("{\"id\":7,\"status\":\"Running\",\"total\":3}")]
    public async Task GetSearchResults_ReadsNullAndLegacyObjectStatus(string statusJson)
    {
        var json = $"{{\"results\":[],\"status\":{statusJson},\"total\":0}}";
        var (netService, httpClient, _) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var result = await new SearchService(netService).GetSearchResults(1);

            if (statusJson == "null")
                That(result!.Status, Is.Null);
            else
                Multiple(() =>
                {
                    That(result!.Status!.Id, Is.EqualTo(7));
                    That(result.Status.Status, Is.EqualTo("Running"));
                    That(result.Status.Count, Is.EqualTo(3));
                });
        }
    }

    [Test]
    public async Task GetApplicationPreferences_DeserializesNewConfirmationFlags()
    {
        const string json = """
            {
              "confirm_remove_all_tags": true,
              "confirm_remove_tracker_from_all_torrents": false,
              "futureField": "preserved"
            }
            """;
        var (netService, httpClient, handler) =
            CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var preferences = await new ApplicationService(netService).GetApplicationPreferences();

            Multiple(() =>
            {
                That(preferences!.ConfirmRemoveAllTags, Is.True);
                That(preferences.ConfirmRemoveTrackerFromAllTorrents, Is.False);
            });
        }
    }

    [Test]
    public async Task ApplicationPreferences_ReadAndWriteNewFieldsAndLeaveMissingFieldsNull()
    {
        var get = CreateNetService(_ => StubHttpMessageHandler.JsonResponse("{}"));
        using (get.NetService)
        using (get.HttpClient)
        {
            var preferences = await new ApplicationService(get.NetService).GetApplicationPreferences();
            Multiple(() =>
            {
                That(preferences!.ConfirmRemoveAllTags, Is.Null);
                That(preferences.ConfirmRemoveTrackerFromAllTorrents, Is.Null);
            });
        }

        var post = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""));
        using (post.NetService)
        using (post.HttpClient)
        {
            await new ApplicationService(post.NetService).SetApplicationPreferences(new ApplicationPreferences
            {
                ConfirmRemoveAllTags = true,
                ConfirmRemoveTrackerFromAllTorrents = false
            });

            var body = DecodeForm(post.Handler.Requests.Single());
            That(body, Does.Contain("confirm_remove_all_tags"));
            That(body, Does.Contain("confirm_remove_tracker_from_all_torrents"));
            That(body, Does.Contain("true"));
            That(body, Does.Contain("false"));
        }
    }

    [Test]
    public async Task SyncMainData_DeserializesErrorAndKeepsMissingErrorNull()
    {
        const string json = """{"rid":1,"torrents":{"abc":{"hash":"abc","error":"tracker failure"},"def":{"hash":"def"}}}""";
        var (netService, httpClient, _) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(json));
        using (netService)
        using (httpClient)
        {
            var data = await new SyncService(netService).GetMainData();
            Multiple(() =>
            {
                That(data!.Torrents!["abc"].Error, Is.EqualTo("tracker failure"));
                That(data.Torrents["def"].Error, Is.Null);
            });
        }
    }

    [Test]
    public async Task CategoryMethods_PreserveOriginalFiveParameterSignaturesAndCalls()
    {
        var createMethod = typeof(TorrentService).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method => method.Name == nameof(TorrentService.CreateCategory) &&
                              method.GetParameters().Length == 5);
        var editMethod = typeof(TorrentService).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method => method.Name == nameof(TorrentService.EditCategory) &&
                              method.GetParameters().Length == 5);
        That(createMethod.GetParameters()[4].ParameterType, Is.EqualTo(typeof(CancellationToken)));
        That(editMethod.GetParameters()[4].ParameterType, Is.EqualTo(typeof(CancellationToken)));
        var expandedCreateMethod = typeof(TorrentService).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method => method.Name == nameof(TorrentService.CreateCategory) &&
                              method.GetParameters().Length == 10);
        var expandedEditMethod = typeof(TorrentService).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method => method.Name == nameof(TorrentService.EditCategory) &&
                              method.GetParameters().Length == 10);
        That(expandedCreateMethod.GetParameters()[4].IsOptional, Is.True);
        That(expandedEditMethod.GetParameters()[4].IsOptional, Is.True);

        var (netService, httpClient, handler) = CreateNetService(_ => StubHttpMessageHandler.JsonResponse(""),
                                                                 Previous);
        using (netService)
        using (httpClient)
        {
            var torrent = new TorrentService(netService, Previous);
            Func<string, string, string?, bool?, CancellationToken, Task> create = torrent.CreateCategory;
            Func<string, string, string?, bool?, CancellationToken, Task> edit = torrent.EditCategory;
            using var canceledSource = new CancellationTokenSource();
            canceledSource.Cancel();
            ThrowsAsync<OperationCanceledException>(async () =>
                await create("cancelled", "/downloads/cancelled", null, null, canceledSource.Token));
            That(handler.Requests, Is.Empty);

            var cancellationToken = CancellationToken.None;

            await create("movies", "/downloads/movies", null, null, cancellationToken);
            await edit("movies", "/downloads/movies", null, null, cancellationToken);

            Multiple(() =>
            {
                That(DecodeForm(handler.Requests[0]), Is.EqualTo("category=movies&savePath=/downloads/movies"));
                That(DecodeForm(handler.Requests[1]), Is.EqualTo("category=movies&savePath=/downloads/movies"));
            });
        }
    }

    private static (NetService NetService, HttpClient HttpClient, StubHttpMessageHandler Handler)
        CreateNetService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory, ApiVersion? apiVersion = null)
    {
        var handler    = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(apiVersion ?? Supported);
        return (netService, httpClient, handler);
    }

    private static string DecodeForm(HttpRequestSnapshot request) =>
        Uri.UnescapeDataString(request.Body ?? string.Empty);
}
