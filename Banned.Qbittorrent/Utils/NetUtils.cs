using System.Net;
using System.Net.Http.Headers;

namespace Banned.Qbittorrent.Utils;

public class NetUtils
{
    private readonly HttpClient      _client;
    private readonly CookieContainer _cookieContainer;
    private readonly Uri             _baseUrl;

    private readonly string _userName;
    private readonly string _password;

    public NetUtils(string baseUrl, string userName, string password)
    {
        _baseUrl         = new Uri(baseUrl.TrimEnd('/') + "/");
        _userName        = userName;
        _password        = password;
        _cookieContainer = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer   = _cookieContainer,
            AllowAutoRedirect = true,
            UseCookies        = true
        };

        _client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true
        };
    }

    /// <summary>
    /// 拼接 baseUrl 和 subPath，确保不会出现多余或缺少 "/"
    /// </summary>
    private Uri CombineUrl(string subPath)
    {
        return new Uri(_baseUrl, subPath.TrimStart('/'));
    }

    /// <summary>
    /// 通用 GET 请求（异步）
    /// </summary>
    public async Task<(HttpStatusCode, string)> GetAsync(string subPath)
    {
        await EnsureLoggedInAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, CombineUrl(subPath));
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    /// <summary>
    /// 通用 POST 请求（异步）
    /// </summary>
    public async Task<(HttpStatusCode, string)> PostAsync(string subPath, Dictionary<string, string> parameters)
    {
        await EnsureLoggedInAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath))
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    /// <summary>
    /// 通用 POST 文件上传（异步）
    /// </summary>
    public async Task<(HttpStatusCode, string)> PostWithFilesAsync(string                     subPath,
                                                                   Dictionary<string, string> parameters,
                                                                   List<string>               filePaths)
    {
        await EnsureLoggedInAsync();

        using var content = new MultipartFormDataContent();
        foreach (var param in parameters)
        {
            content.Add(new StringContent(param.Value), param.Key);
        }

        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                content.Add(new ByteArrayContent(await File.ReadAllBytesAsync(filePath)), "torrents",
                            Path.GetFileName(filePath));
            }
            else
            {
                Console.WriteLine($"文件未找到: {filePath}");
                return (HttpStatusCode.BadRequest, $"File not found: {filePath}");
            }
        }

        var request = new HttpRequestMessage(HttpMethod.Post, CombineUrl(subPath)) { Content = content };
        request.Headers.ConnectionClose = true;

        return await ExecuteWithRetryAsync(() => _client.SendAsync(request));
    }

    private async Task EnsureLoggedInAsync()
    {
        if (_cookieContainer.GetCookies(_baseUrl).Count > 0) return;
        await LoginAsync();
    }

    private async Task LoginAsync()
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "username", _userName },
            { "password", _password }
        });

        var response = await _client.PostAsync(CombineUrl("api/v2/auth/login"), content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Login failed: {response.StatusCode}");
        }
    }

    /// <summary>
    /// 自动重试逻辑（异步）
    /// </summary>
    private static async Task<(HttpStatusCode, string)> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action,
                                                                              int maxRetries = 3)
    {
        for (var retry = 0; retry < maxRetries; retry++)
        {
            var response = await action();
            if (response.StatusCode != HttpStatusCode.InternalServerError)
                return (response.StatusCode, await response.Content.ReadAsStringAsync());

            await Task.Delay(500 * (retry + 1));
        }

        return (HttpStatusCode.InternalServerError, "Max retry limit reached.");
    }
}
