using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Services;
using System.Reflection;
using static NUnit.Framework.Assert;

namespace Banned.Qbittorrent.Test;

[TestFixture]
public class CancellationTokenTests
{
    private static readonly Type[] PublicNetworkTypes =
    [
        typeof(QBittorrentClient),
        typeof(ApplicationService),
        typeof(AuthenticationService),
        typeof(LogService),
        typeof(NetService),
        typeof(RssService),
        typeof(SearchService),
        typeof(SyncService),
        typeof(TorrentCreatorService),
        typeof(TorrentService),
        typeof(TransferService)
    ];

    [Test]
    public void PublicAsyncNetworkMethods_PreserveCancellationTokenConventions()
    {
        var offenders = PublicNetworkTypes
                       .SelectMany(type => type.GetMethods(BindingFlags.Public |
                                                           BindingFlags.Instance |
                                                           BindingFlags.Static |
                                                           BindingFlags.DeclaredOnly))
                       .Where(method => typeof(Task).IsAssignableFrom(method.ReturnType))
                       .Where(method =>
                       {
                           var parameters = method.GetParameters();
                           // Category overloads retain the original token position for caller compatibility.
                           if (method.DeclaringType == typeof(TorrentService) &&
                               method.Name is nameof(TorrentService.CreateCategory) or
                                              nameof(TorrentService.EditCategory))
                               return parameters.Length is not (5 or 10) ||
                                      parameters[4].ParameterType != typeof(CancellationToken) ||
                                      (parameters.Length == 10 &&
                                       parameters.Skip(4).Any(parameter => !parameter.IsOptional));

                           return parameters.Length == 0 ||
                                  parameters[^1].ParameterType != typeof(CancellationToken) ||
                                  !parameters[^1].IsOptional;
                       })
                       .Select(method => $"{method.DeclaringType!.Name}.{method.Name}")
                       .Order()
                       .ToArray();

        That(offenders, Is.Empty,
             "Network methods must end with an optional cancellation token, except category overloads that retain the original fifth position.");
    }

    [Test]
    public async Task ServiceMethod_PassesCallerTokenToHttpHandler()
    {
        var handler = new CancellationObservingHandler();
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_15_1);
        using var cancellation = new CancellationTokenSource();

        var request = new ApplicationService(netService).GetVersion(cancellation.Token);
        await handler.RequestStarted.Task;
        cancellation.Cancel();

        ThrowsAsync<TaskCanceledException>(async () => await request);
    }

    [Test]
    public void ConvenienceMethod_PropagatesCanceledTokenWithoutSendingRequest()
    {
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse(string.Empty));
        using var httpClient = new HttpClient(handler);
        using var netService = new NetService("http://localhost:8080", httpClient);
        netService.SetApiVersion(ApiVersion.V2_15_1);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        ThrowsAsync<OperationCanceledException>(async () =>
            await new TorrentService(netService, ApiVersion.V2_15_1)
               .PauseTorrents(["abc"], cancellation.Token));

        That(handler.Requests, Is.Empty);
    }

    [Test]
    public void ClientCreate_HonorsCancellationBeforeLoginRequest()
    {
        var handler = new StubHttpMessageHandler(_ => StubHttpMessageHandler.JsonResponse("Ok."));
        using var httpClient = new HttpClient(handler);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        ThrowsAsync<TaskCanceledException>(async () =>
            await QBittorrentClient.Create(
                "http://localhost:8080",
                "user",
                "password",
                httpClient : httpClient,
                cancellationToken : cancellation.Token));

        That(handler.Requests, Is.Empty);
    }

    private sealed class CancellationObservingHandler : HttpMessageHandler
    {
        public TaskCompletionSource RequestStarted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestStarted.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            throw new InvalidOperationException("The cancellation token did not cancel the HTTP request.");
        }
    }
}
