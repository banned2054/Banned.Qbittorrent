using Banned.Qbittorrent.Exceptions;
using Banned.Qbittorrent.Models;
using Banned.Qbittorrent.Models.Application;
using Banned.Qbittorrent.Models.Enums;
using Banned.Qbittorrent.Utils;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Banned.Qbittorrent.Utils.NetUtils;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供底层的 HTTP 网络请求服务，包含自动重试、身份验证检查和版本控制。<br/>
/// Provides underlying HTTP networking services, including automatic retries, authentication checks, and version control.
/// </summary>
public class NetService : IDisposable
{
    private static long _requestSequence;

    private readonly Func<AddressFamilyPreference, HttpClient>?         _httpClientFactory;
    private readonly Func<string, CancellationToken, Task<IPAddress[]>> _addressResolver;
    private readonly Action<string>?                                    _diagnosticSink;
    private readonly List<HttpClient>                                   _retiredClients = [];
    private readonly CookieContainer?                                   _cookieContainer;

    private readonly Uri      _baseUrl;
    private readonly TimeSpan _configuredTimeout;
    private readonly TimeSpan _connectTimeout;
    private readonly object   _clientRefreshLock = new();
    private readonly bool     _ownsHttpClient;
    private readonly bool     _enableAutomaticIPv4Fallback;

    private AddressFamilyPreference _addressFamilyPreference;

    private int  _clientGeneration;
    private bool _automaticFallbackAttempted;
    private bool _disposed;

    private HttpClient _client;

    private ApiVersion _apiVersion;

    /// <summary>
    /// 获取或设置用于确保用户已登录的处理程序。<br/>
    /// Gets or sets the handler used to ensure the user is logged in.
    /// </summary>
    public Func<Task>? EnsureLoggedInHandler { get; set; }

    /// <summary>
    /// 获取或设置用于确保用户已登录的处理程序，可指定是否强制重新登录。<br/>
    /// Gets or sets the handler used to ensure the user is logged in, with optional forced re-login.
    /// </summary>
    public Func<bool, CancellationToken, Task>? EnsureLoggedInAsyncHandler { get; set; }

    /// <summary>
    /// 获取或设置最大重试次数。<br/>
    /// Gets or sets the maximum number of attempts. The legacy property name is retained for compatibility.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 获取默认请求超时时间。<br/>
    /// Gets the default request timeout.
    /// </summary>
    public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(15);

    /// <summary>
    /// 获取默认 TCP 连接建立超时时间。<br/>
    /// Gets the default TCP connection establishment timeout.
    /// </summary>
    public static readonly TimeSpan DefaultConnectTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 获取或设置是否在网络故障异常中包含额外诊断信息。<br/>
    /// Gets or sets whether network failure exceptions include additional diagnostics.
    /// </summary>
    public bool EnableDetailedLogging { get; set; }

    private bool DetailedDiagnosticsEnabled => EnableDetailedLogging || _diagnosticSink != null;

    /// <summary>
    /// 初始化 <see cref="NetService"/> 类的新实例。<br/>
    /// Initializes a new instance of the <see cref="NetService"/> class.
    /// </summary>
    /// <param name="baseUrl">qBittorrent Web UI 的基础地址。 / The base URL of qBittorrent Web UI.</param>
    /// <param name="httpClient">可选的 HttpClient 实例。 / Optional HttpClient instance.</param>
    /// <param name="timeout">可选的请求超时时间，为 null 时默认 15 秒。 / Optional request timeout, default 15 seconds when null.</param>
    public NetService(string baseUrl, HttpClient? httpClient = null, TimeSpan? timeout = null) : this(baseUrl,
        new QBittorrentClientOptions { HttpClient = httpClient, Timeout = timeout ?? DefaultRequestTimeout })
    {
    }

    internal NetService(string baseUrl, QBittorrentClientOptions options) : this(baseUrl, options, null, null)
    {
    }

    internal NetService(
        string                                              baseUrl,
        QBittorrentClientOptions                            options,
        Func<string, CancellationToken, Task<IPAddress[]>>? addressResolver,
        Func<AddressFamilyPreference, HttpClient>?          httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentNullException.ThrowIfNull(options);
        NetUtils.ValidateOptions(options);

        _baseUrl                     = new Uri(baseUrl.TrimEnd('/') + "/");
        _configuredTimeout           = options.Timeout;
        _connectTimeout              = options.ConnectTimeout;
        _addressResolver             = addressResolver ?? HttpClientUtils.ResolveHostAddressesAsync;
        _diagnosticSink              = options.DiagnosticSink;
        _enableAutomaticIPv4Fallback = options.EnableAutomaticIPv4Fallback;
        _addressFamilyPreference     = options.AddressFamilyPreference;
        MaxRetries                   = options.MaxRetries;
        EnableDetailedLogging        = options.EnableDetailedLogging;

        if (options.HttpClient != null)
        {
            _client         = options.HttpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _cookieContainer = new CookieContainer();
            _ownsHttpClient  = true;
            _httpClientFactory = httpClientFactory ?? (preference => HttpClientUtils.CreateDefaultHttpClient(
             _cookieContainer,
             _configuredTimeout,
             _connectTimeout,
             preference,
             _addressResolver,
             _diagnosticSink == null ? null : EmitConnectionDiagnostic));
            _client = CreateDefaultHttpClient(_addressFamilyPreference);
        }
    }

    private HttpClient CreateDefaultHttpClient(AddressFamilyPreference preference) =>
        _httpClientFactory!(preference);

    private async Task<bool> TryUpgradeToIPv4Fallback(
        Exception         exception,
        HttpMethod?       method,
        bool              isAuthenticationEndpoint,
        StringBuilder     diagnostics,
        long              requestId,
        CancellationToken cancellationToken)
    {
        if (!_ownsHttpClient || _cookieContainer == null)
        {
            AppendDiagnostic(
                             diagnostics, requestId,
                             "automatic IPv4 fallback skipped because HttpClient is caller-owned");
            return false;
        }

        if (!_enableAutomaticIPv4Fallback                              ||
            _addressFamilyPreference != AddressFamilyPreference.System ||
            _automaticFallbackAttempted                                ||
            !NetUtils.IsConnectionEstablishmentFailure(exception)      ||
            !NetUtils.IsSafeToReplayAfterConnectionFailure(method, isAuthenticationEndpoint))
            return false;

        var addresses = await ResolveAddressesForFallback(diagnostics, requestId, cancellationToken)
           .ConfigureAwait(false);
        var hasIPv4 = addresses.Any(address => address.AddressFamily == AddressFamily.InterNetwork);
        var hasIPv6 = addresses.Any(address => address.AddressFamily == AddressFamily.InterNetworkV6);
        if (!hasIPv4 || !hasIPv6)
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"automatic IPv4 fallback skipped because DNS is not dual-stack; hasIPv4={hasIPv4} hasIPv6={hasIPv6}");
            return false;
        }

        HttpClient? replacement = null;
        HttpClient? previous    = null;
        var         upgraded    = false;
        lock (_clientRefreshLock)
        {
            if (!_automaticFallbackAttempted && _addressFamilyPreference == AddressFamilyPreference.System)
            {
                _automaticFallbackAttempted = true;
                replacement                 = CreateDefaultHttpClient(AddressFamilyPreference.PreferIPv4);
                previous                    = _client;
                Volatile.Write(ref _client, replacement);
                _retiredClients.Add(previous);
                _addressFamilyPreference = AddressFamilyPreference.PreferIPv4;
                _clientGeneration++;
                upgraded = true;
            }
        }

        if (upgraded)
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"automatic IPv4 fallback activated oldPolicy={AddressFamilyPreference.System} " +
                             $"newPolicy={AddressFamilyPreference.PreferIPv4} generation={_clientGeneration} " +
                             $"oldHttpClientId={previous!.GetHashCode()} newHttpClientId={replacement!.GetHashCode()} " +
                             $"trigger={DescribeException(exception)}");
        }
        else
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"automatic IPv4 fallback already activated by another request; generation={_clientGeneration}");
        }

        return true;
    }

    private async Task<IPAddress[]> ResolveAddressesForFallback(
        StringBuilder     diagnostics,
        long              requestId,
        CancellationToken cancellationToken)
    {
        if (Uri.CheckHostName(_baseUrl.Host) != UriHostNameType.Dns)
            return [];

        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(3));
            var addresses = await _addressResolver(_baseUrl.DnsSafeHost, timeoutCts.Token).ConfigureAwait(false);
            if (DetailedDiagnosticsEnabled)
            {
                AppendDiagnostic(diagnostics, requestId,
                                 $"fallback DNS candidates=[{string.Join(", ", addresses.Select(address => $"{address.AddressFamily}:{address}"))}]");
            }

            return addresses;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            AppendDiagnostic(diagnostics, requestId, "automatic IPv4 fallback DNS check timed out");
            return [];
        }
        catch (Exception exception) when (exception is SocketException or HttpRequestException)
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"automatic IPv4 fallback DNS check failed: {DescribeException(exception)}");
            return [];
        }
    }

    /// <summary>
    /// 设置当前客户端使用的 Web API 版本。<br/>
    /// Sets the Web API version used by the current client.
    /// </summary>
    /// <param name="apiVersion">API 版本信息。 / API version information.</param>
    public void SetApiVersion(ApiVersion apiVersion) => _apiVersion = apiVersion;

    private void EnsureApiVersionSupported(string endpoint, ApiVersionRange versionRange)
    {
        if (versionRange.Introduced is { } introduced && _apiVersion < introduced)
            throw new QbittorrentNotSupportedException(endpoint, introduced, _apiVersion);

        if (versionRange.Removed is { } removed && _apiVersion >= removed)
            throw new QbittorrentEndpointRemovedException(endpoint, removed, _apiVersion);
    }

    private Uri CombineUrl(string subPath) => new(_baseUrl, subPath.TrimStart('/'));

    /// <summary>
    /// 发起异步 GET 请求。<br/>
    /// Performs an asynchronous GET request.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="targetVersion">该接口要求的最低 API 版本。 / The minimum API version required by this endpoint.</param>
    /// <param name="opName">操作名称，用于异常显示。 / The operation name used for exception display.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip the authentication check.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">覆盖此请求的最大重试次数。 / Maximum retry count override for this request.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    /// <exception cref="QbittorrentNotSupportedException">当前 API 版本低于目标版本时抛出。 / Thrown when the current API version is lower than the target version.</exception>
    public async Task<string> Get(string            subPath,
                                  ApiVersion?       targetVersion = null,
                                  string?           opName        = null,
                                  bool              skipAuthCheck = false,
                                  CancellationToken ct            = default,
                                  int?              maxRetries    = null)
    {
        EnsureApiVersionSupported(opName ?? subPath, new ApiVersionRange(targetVersion));

        if (!skipAuthCheck && !IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() => new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath)), maxRetries, ct,
                                      skipAuthRetry : IsAuthenticationEndpoint(subPath))
           .ConfigureAwait(false);
    }

    /// <summary>
    /// 发起具有完整版本区间约束的异步 GET 请求。<br/>
    /// Performs an asynchronous GET request constrained by a complete API version range.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="versionRange">端点有效版本区间。 / Endpoint availability range.</param>
    /// <param name="opName">操作名称。 / Operation name.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip authentication checks.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">最大重试次数覆盖值。 / Maximum retry count override.</param>
    /// <returns>响应体字符串。 / Response body string.</returns>
    public async Task<string> Get(string subPath,               ApiVersionRange   versionRange, string? opName = null,
                                  bool   skipAuthCheck = false, CancellationToken ct = default, int? maxRetries = null)
    {
        EnsureApiVersionSupported(opName ?? subPath, versionRange);

        if (!skipAuthCheck && !IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() => new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath)), maxRetries, ct,
                                      skipAuthRetry : IsAuthenticationEndpoint(subPath)).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起异步 POST 请求。<br/>
    /// Performs an asynchronous POST request.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">表单参数。 / Form parameters.</param>
    /// <param name="targetVersion">该接口要求的最低 API 版本。 / The minimum API version required by this endpoint.</param>
    /// <param name="opName">操作名称，用于异常显示。 / The operation name used for exception display.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip the authentication check.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">覆盖此请求的最大重试次数。 / Maximum retry count override for this request.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    public async Task<string> Post(string                      subPath,
                                   Dictionary<string, string>? parameters    = null,
                                   ApiVersion?                 targetVersion = null,
                                   string?                     opName        = null,
                                   bool                        skipAuthCheck = false,
                                   CancellationToken           ct            = default,
                                   int?                        maxRetries    = null)
    {
        EnsureApiVersionSupported(opName ?? subPath, new ApiVersionRange(targetVersion));

        if (!skipAuthCheck && !IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath));

            if (parameters != null)
            {
                request.Content = new FormUrlEncodedContent(parameters);
            }

            return request;
        }, maxRetries, ct, skipAuthRetry : IsAuthenticationEndpoint(subPath)).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起具有完整版本区间约束的异步 POST 请求。<br/>
    /// Performs an asynchronous POST request constrained by a complete API version range.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">表单参数。 / Form parameters.</param>
    /// <param name="versionRange">端点有效版本区间。 / Endpoint availability range.</param>
    /// <param name="opName">操作名称。 / Operation name.</param>
    /// <param name="skipAuthCheck">是否跳过登录状态检查。 / Whether to skip authentication checks.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">最大重试次数覆盖值。 / Maximum retry count override.</param>
    /// <returns>响应体字符串。 / Response body string.</returns>
    public async Task<string> Post(string                      subPath,
                                   Dictionary<string, string>? parameters,
                                   ApiVersionRange             versionRange,
                                   string?                     opName        = null,
                                   bool                        skipAuthCheck = false,
                                   CancellationToken           ct            = default,
                                   int?                        maxRetries    = null)
    {
        EnsureApiVersionSupported(opName ?? subPath, versionRange);

        if (!skipAuthCheck && !IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            var request                             = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath));
            if (parameters != null) request.Content = new FormUrlEncodedContent(parameters);
            return request;
        }, maxRetries, ct, skipAuthRetry : IsAuthenticationEndpoint(subPath)).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起异步 POST 请求并以字节数组读取响应。<br/>
    /// Performs an asynchronous POST request and reads the response as a byte array.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">表单参数。 / Form parameters.</param>
    /// <param name="targetVersion">该接口要求的最低 API 版本。 / The minimum API version required by this endpoint.</param>
    /// <param name="opName">操作名称，用于异常显示。 / The operation name used for exception display.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字节数组。 / The response body byte array.</returns>
    public async Task<byte[]> PostBytes(string                      subPath,
                                        Dictionary<string, string>? parameters    = null,
                                        ApiVersion?                 targetVersion = null,
                                        string?                     opName        = null,
                                        CancellationToken           ct            = default)
    {
        EnsureApiVersionSupported(opName ?? subPath, new ApiVersionRange(targetVersion));

        if (!IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
                                      {
                                          var request =
                                              new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath));
                                          if (parameters != null)
                                              request.Content = new FormUrlEncodedContent(parameters);
                                          return Task.FromResult(request);
                                      },
                                      static (content, cancellationToken) =>
                                          content.ReadAsByteArrayAsync(cancellationToken), ct : ct,
                                      skipAuthRetry : IsAuthenticationEndpoint(subPath)).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起具有完整版本区间约束的异步 POST 请求并读取字节响应。<br/>
    /// Performs an asynchronous POST request constrained by a complete API version range and reads bytes.
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">表单参数。 / Form parameters.</param>
    /// <param name="versionRange">端点有效版本区间。 / Endpoint availability range.</param>
    /// <param name="opName">操作名称。 / Operation name.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字节数组。 / Response body byte array.</returns>
    public async Task<byte[]> PostBytes(string                      subPath,
                                        Dictionary<string, string>? parameters,
                                        ApiVersionRange             versionRange,
                                        string?                     opName = null,
                                        CancellationToken           ct     = default)
    {
        EnsureApiVersionSupported(opName ?? subPath, versionRange);

        if (!IsAuthenticationEndpoint(subPath))
            await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
                                      {
                                          var request =
                                              new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath));
                                          if (parameters != null)
                                              request.Content = new FormUrlEncodedContent(parameters);
                                          return Task.FromResult(request);
                                      },
                                      static (content, cancellationToken) =>
                                          content.ReadAsByteArrayAsync(cancellationToken), ct : ct,
                                      skipAuthRetry : IsAuthenticationEndpoint(subPath)).ConfigureAwait(false);
    }

    /// <summary>
    /// 发起带文件的异步 POST 请求（多部分表单）。<br/>
    /// Performs an asynchronous POST request with files (multipart/form-data).
    /// </summary>
    /// <param name="subPath">请求的子路径。 / The sub-path of the request.</param>
    /// <param name="parameters">额外的表单字段。 / Additional form fields.</param>
    /// <param name="filePaths">要上传的文件路径列表。 / List of file paths to upload.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串。 / The response body string.</returns>
    /// <exception cref="QbittorrentFileNotFoundException">当指定的文件不存在时抛出。 / Thrown when a specified file does not exist.</exception>
    public async Task<string> PostWithFiles(string                      subPath,
                                            Dictionary<string, string>? parameters,
                                            List<string>                filePaths,
                                            CancellationToken           ct = default)
    {
        await EnsureLoggedIn(force : false, ct).ConfigureAwait(false);

        return await ExecuteWithRetry(() =>
        {
            try
            {
                var content = new MultipartFormDataContent();
                if (parameters != null)
                    foreach (var (k, v) in parameters)
                        content.Add(new StringContent(v), k);

                foreach (var filePath in filePaths)
                {
                    if (!File.Exists(filePath)) throw new QbittorrentFileNotFoundException(filePath);

                    // 使用 FileStream 代替 File.ReadAllBytes 以减少内存使用
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192,
                                                    FileOptions.Asynchronous);
                    try
                    {
                        content.Add(new StreamContent(fileStream), "torrents", Path.GetFileName(filePath));
                    }
                    catch
                    {
                        fileStream.Dispose();
                        throw;
                    }
                }

                var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };

                return Task.FromResult(request);
            }
            catch (Exception exception)
            {
                return Task.FromException<HttpRequestMessage>(exception);
            }
        }, null, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// 并行执行多个请求。<br/>
    /// Executes multiple requests in parallel.
    /// </summary>
    /// <param name="requests">请求信息列表。 / List of request information.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <returns>响应体字符串列表，与请求顺序对应。 / List of response body strings, corresponding to the request order.</returns>
    public async Task<List<string>> ExecuteParallelRequests(
        List<(string subPath, HttpMethod method, Dictionary<string, string>? parameters)> requests,
        CancellationToken                                                                 ct = default)
    {
        var tasks = requests.Select(async request =>
        {
            if (request.method == HttpMethod.Get)
            {
                return await Get(request.subPath, skipAuthCheck : false, ct : ct);
            }

            return await Post(request.subPath, request.parameters, skipAuthCheck : false, ct : ct);
        }).ToList();

        var results = await Task.WhenAll(tasks);
        return [.. results];
    }

    /// <summary>
    /// 执行请求并包含指数退避重试机制。<br/>
    /// Executes the request with an exponential backoff retry mechanism.
    /// </summary>
    /// <param name="requestFactory">用于创建请求消息的工厂方法。 / Factory method to create the request message.</param>
    /// <param name="ct">取消令牌。 / Cancellation token.</param>
    /// <param name="maxRetries">最大重试次数。 / Maximum number of retries.</param>
    /// <param name="skipAuthRetry">是否跳过身份验证失败后的重新登录重试。 / Whether to skip the login retry after an authentication failure.</param>
    /// <returns>响应体内容。 / Response body content.</returns>
    private async Task<string> ExecuteWithRetry(Func<HttpRequestMessage> requestFactory,
                                                int?                     maxRetries    = null,
                                                CancellationToken        ct            = default,
                                                bool                     skipAuthRetry = false)
    {
        return await ExecuteWithRetry(() =>
        {
            try
            {
                return Task.FromResult(requestFactory());
            }
            catch (Exception exception)
            {
                return Task.FromException<HttpRequestMessage>(exception);
            }
        }, maxRetries, ct, skipAuthRetry);
    }

    private async Task<string> ExecuteWithRetry(Func<Task<HttpRequestMessage>> requestFactory,
                                                int?                           maxRetries    = null,
                                                CancellationToken              ct            = default,
                                                bool                           skipAuthRetry = false)
    {
        return await ExecuteWithRetry(requestFactory,
                                      static (content, cancellationToken) =>
                                          content.ReadAsStringAsync(cancellationToken), maxRetries, ct, skipAuthRetry)
           .ConfigureAwait(false);
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<HttpRequestMessage>>                requestFactory,
                                              Func<HttpContent, CancellationToken, Task<T>> responseReader,
                                              int?                                          maxRetries    = null,
                                              CancellationToken                             ct            = default,
                                              bool                                          skipAuthRetry = false)
    {
        Exception?           lastException     = null;
        HttpResponseMessage? lastResponse      = null;
        HttpMethod?          lastRequestMethod = null;

        var lastBody           = string.Empty;
        var actualMaxRetries   = maxRetries ?? MaxRetries;
        var lastRequestInfo    = "unknown request";
        var authRetryAttempted = false;
        var requestId          = Interlocked.Increment(ref _requestSequence);
        var totalStopwatch     = Stopwatch.StartNew();
        var diagnostics        = new StringBuilder();

        AppendDiagnostic(diagnostics, requestId,
                         $"BEGIN maxRetries={actualMaxRetries} configuredMaxRetries={MaxRetries} " +
                         $"overrideMaxRetries={(maxRetries.HasValue ? maxRetries.Value.ToString() : "<null>")} " +
                         $"httpClientTimeout={_client.Timeout} ownsHttpClient={_ownsHttpClient} " +
                         $"configuredTimeout={_configuredTimeout} connectTimeout={_connectTimeout} " +
                         $"addressFamilyPreference={_addressFamilyPreference} generation={_clientGeneration} " +
                         $"skipAuthRetry={skipAuthRetry} ctCanBeCanceled={ct.CanBeCanceled} " +
                         $"baseUrl={RedactUri(_baseUrl)} endpoint={DescribeEndpoint(_baseUrl)} machine={Environment.MachineName} " +
                         $"framework={System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} " +
                         $"os={System.Runtime.InteropServices.RuntimeInformation.OSDescription} " +
                         $"processId={Environment.ProcessId} threadId={Environment.CurrentManagedThreadId}");

        if (DetailedDiagnosticsEnabled) await AppendDnsDiagnostic(diagnostics, requestId, ct).ConfigureAwait(false);

        for (var attempt = 1; attempt <= actualMaxRetries; attempt++)
        {
            var attemptStopwatch = Stopwatch.StartNew();
            try
            {
                AppendDiagnostic(diagnostics, requestId, $"attempt {attempt}/{actualMaxRetries}: creating request");
                using var request = await requestFactory().ConfigureAwait(false);
                lastRequestInfo   = $"{request.Method} {RedactUri(request.RequestUri)}";
                lastRequestMethod = request.Method;
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: sending {DescribeRequest(request)}");

                var client = Volatile.Read(ref _client);
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: using httpClientId={client.GetHashCode()} " +
                                 $"timeout={client.Timeout} addressFamilyPreference={_addressFamilyPreference} "     +
                                 $"generation={_clientGeneration}");

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
                                                 .ConfigureAwait(false);

                lastResponse = response;

                if (response.IsSuccessStatusCode)
                {
                    var result = await responseReader(response.Content, ct).ConfigureAwait(false);
                    var responseDescription = result is string body
                        ? body
                        : $"<{typeof(T).Name}; contentLength={response.Content.Headers.ContentLength?.ToString() ?? "unknown"}>";

                    AppendDiagnostic(diagnostics, requestId,
                                     $"attempt {attempt}/{actualMaxRetries}: response after {attemptStopwatch.Elapsed} " +
                                     $"{DescribeResponse(response, responseDescription)}");
                    return result;
                }

                lastBody = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: response after {attemptStopwatch.Elapsed} " +
                                 $"{DescribeResponse(response, lastBody)}");

                if (!skipAuthRetry && !authRetryAttempted && IsAuthenticationFailure(response.StatusCode) &&
                    HasEnsureLoggedInHandler())
                {
                    authRetryAttempted = true;
                    AppendDiagnostic(diagnostics, requestId,
                                     $"attempt {attempt}/{actualMaxRetries}: authentication failure " +
                                     $"{response.StatusCode}; forcing login and retrying same attempt");

                    await EnsureLoggedIn(force : true, ct).ConfigureAwait(false);
                    attempt--;
                    continue;
                }

                if (!IsRetryableStatusCode(response.StatusCode))
                    throw MapToException(response, lastBody);

                if (attempt == actualMaxRetries) break;

                var delay = GetRetryDelay(response, attempt);
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: retrying after {delay} because status={response.StatusCode}");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (TaskCanceledException ex)
            {
                if (ct.IsCancellationRequested)
                    throw;

                lastException = ex;
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: canceled after {attemptStopwatch.Elapsed} " +
                                 $"request={lastRequestInfo} exception={DescribeException(ex)}");
                if (attempt == actualMaxRetries) break;

                var fallbackActivated =
                    await TryUpgradeToIPv4Fallback(ex, lastRequestMethod, skipAuthRetry, diagnostics, requestId, ct)
                       .ConfigureAwait(false);

                if (!fallbackActivated &&
                    !IsSafeToReplayAfterConnectionFailure(lastRequestMethod, skipAuthRetry))
                {
                    AppendDiagnostic(diagnostics, requestId,
                                     $"attempt {attempt}/{actualMaxRetries}: retry suppressed because {lastRequestMethod} may have side effects");
                    break;
                }

                var delay = ComputeBackoff(attempt);
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: retrying after {delay} due to cancellation");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: http request failed after {attemptStopwatch.Elapsed} " +
                                 $"request={lastRequestInfo} exception={DescribeException(ex)}");
                if (attempt == actualMaxRetries) break;

                var fallbackActivated =
                    await TryUpgradeToIPv4Fallback(ex, lastRequestMethod, skipAuthRetry, diagnostics, requestId, ct)
                       .ConfigureAwait(false);

                if (!fallbackActivated && !IsSafeToReplayAfterConnectionFailure(lastRequestMethod, skipAuthRetry))
                {
                    AppendDiagnostic(
                                     diagnostics,
                                     requestId,
                                     $"attempt {attempt}/{actualMaxRetries}: retry suppressed because {lastRequestMethod} may have side effects");
                    break;
                }

                var delay = ComputeBackoff(attempt);
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: retrying after {delay} due to network error");

                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 捕获其他异常，不进行重试
                AppendDiagnostic(diagnostics, requestId,
                                 $"attempt {attempt}/{actualMaxRetries}: unexpected exception after {attemptStopwatch.Elapsed} " +
                                 $"request={lastRequestInfo} exception={DescribeException(ex)}");
                throw;
            }
        }

        if (lastResponse != null) throw MapToException(lastResponse, lastBody);
        AppendDiagnostic(diagnostics, requestId,
                         $"FAILED totalElapsed={totalStopwatch.Elapsed} lastRequest={lastRequestInfo} " +
                         $"lastException={DescribeException(lastException)}");
        throw new
            QbittorrentServerErrorException($"Network error after {actualMaxRetries} attempts while sending {lastRequestInfo}: {lastException?.Message ?? "unknown error"}" +
                                            Environment.NewLine + "qBittorrent network diagnostic timeline:" +
                                            Environment.NewLine + diagnostics, lastException);
    }

    private bool HasEnsureLoggedInHandler() => EnsureLoggedInAsyncHandler != null || EnsureLoggedInHandler != null;

    private async Task EnsureLoggedIn(bool force, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (EnsureLoggedInAsyncHandler != null)
        {
            await EnsureLoggedInAsyncHandler.Invoke(force, ct).ConfigureAwait(false);
        }
        else if (EnsureLoggedInHandler != null)
        {
            await EnsureLoggedInHandler.Invoke().ConfigureAwait(false);
        }
    }

    private void AppendDiagnostic(StringBuilder diagnostics, long requestId, string message)
    {
        var line = $"[{DateTimeOffset.Now:O}] [QBT-NET #{requestId}] {message}";
        diagnostics.AppendLine(line);
        EmitDiagnostic(line);
    }

    private void EmitConnectionDiagnostic(string message) =>
        EmitDiagnostic($"[{DateTimeOffset.Now:O}] [QBT-NET] {message}");

    private void EmitDiagnostic(string message)
    {
        try
        {
            _diagnosticSink?.Invoke(message);
        }
        catch
        {
            // Diagnostics must never alter request behavior.
        }
    }

    private async Task AppendDnsDiagnostic(StringBuilder diagnostics, long requestId, CancellationToken ct)
    {
        if (Uri.CheckHostName(_baseUrl.Host) != UriHostNameType.Dns)
        {
            AppendDiagnostic(
                             diagnostics, requestId,
                             $"DNS skipped because endpoint host is {_baseUrl.HostNameType}; address={_baseUrl.Host}");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(3));

            var addresses = await _addressResolver(_baseUrl.DnsSafeHost, timeoutCts.Token).ConfigureAwait(false);
            var resolvedAddresses = addresses.Length == 0
                ? "<none>"
                : string.Join(", ", addresses.Select(address => $"{address.AddressFamily}:{address}"));

            AppendDiagnostic(
                             diagnostics, requestId,
                             $"DNS resolved after {stopwatch.Elapsed}; hostLength={_baseUrl.DnsSafeHost.Length} " +
                             $"addresses=[{resolvedAddresses}]");
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"DNS diagnostic timed out after {stopwatch.Elapsed}; request will continue normally");
        }
        catch (Exception ex)
        {
            AppendDiagnostic(diagnostics, requestId,
                             $"DNS diagnostic failed after {stopwatch.Elapsed}; exception={DescribeException(ex)}; " +
                             "request will continue normally");
        }
    }

    /// <summary>
    /// 释放 <see cref="NetService"/> 使用的资源。<br/>
    /// Releases the resources used by the <see cref="NetService"/>.
    /// </summary>
    public void Dispose()
    {
        lock (_clientRefreshLock)
        {
            if (_disposed) return;
            _disposed = true;

            if (!_ownsHttpClient)
            {
                GC.SuppressFinalize(this);
                return;
            }

            _client.Dispose();
            foreach (var client in _retiredClients)
                client.Dispose();

            _retiredClients.Clear();
        }

        GC.SuppressFinalize(this);
    }
}
