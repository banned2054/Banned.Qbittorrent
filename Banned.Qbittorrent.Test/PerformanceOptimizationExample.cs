using Banned.Qbittorrent.Services;

namespace Banned.Qbittorrent.Test;

public class PerformanceOptimizationExample
{
    /// <summary>
    /// 演示如何使用自定义 HttpClient 优化连接管理
    /// </summary>
    public static async Task UseCustomHttpClientExample()
    {
        // 1. 创建自定义 HttpClient
        var httpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            UseCookies = true
        })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        // 配置默认请求头
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Banned.Qbittorrent SDK");

        // 2. 使用自定义 HttpClient 创建客户端
        var url  = Environment.GetEnvironmentVariable("QBT_URL")  ?? "http://localhost:8080";
        var user = Environment.GetEnvironmentVariable("QBT_USER") ?? "admin";
        var pass = Environment.GetEnvironmentVariable("QBT_PASS") ?? "adminadmin";

        var client = await QBittorrentClient.Create(url, user, pass, httpClient);

        try
        {
            // 3. 使用客户端
            var version = await client.Application.GetVersion();
            Console.WriteLine($"qBittorrent version: {version}");
        }
        finally
        {
            // 4. 清理
            client.Dispose();
            httpClient.Dispose();
        }
    }

    /// <summary>
    /// 演示如何使用并行请求提高性能
    /// </summary>
    public static async Task UseParallelRequestsExample()
    {
        // 1. 创建客户端
        var url  = Environment.GetEnvironmentVariable("QBT_URL")  ?? "http://localhost:8080";
        var user = Environment.GetEnvironmentVariable("QBT_USER") ?? "admin";
        var pass = Environment.GetEnvironmentVariable("QBT_PASS") ?? "adminadmin";

        var client = await QBittorrentClient.Create(url, user, pass);

        try
        {
            // 2. 准备并行请求
            var requests = new List<(string subPath, HttpMethod method, Dictionary<string, string>? parameters)>()
            {
                ($"/api/v2/app/version", HttpMethod.Get, null),
                ($"/api/v2/app/preferences", HttpMethod.Get, null),
                ($"/api/v2/torrents/categories", HttpMethod.Get, null)
            };

            // 3. 执行并行请求
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var netServiceField = client.Application.GetType()
                                        .GetField("netService",
                                                  System.Reflection.BindingFlags.NonPublic |
                                                  System.Reflection.BindingFlags.Instance);
            if (netServiceField == null)
            {
                throw new Exception("无法获取 NetService 实例");
            }

            var netService = netServiceField.GetValue(client.Application) as NetService;
            if (netService == null)
            {
                throw new Exception("无法获取 NetService 实例");
            }

            var results = await netService.ExecuteParallelRequests(requests);
            stopwatch.Stop();

            // 4. 处理结果
            Console.WriteLine($"并行请求执行时间: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"版本信息: {results[0]}");
            Console.WriteLine($"偏好设置长度: {results[1].Length} 字符");
            Console.WriteLine($"分类信息: {results[2]}");
        }
        finally
        {
            client.Dispose();
        }
    }

    /// <summary>
    /// 运行所有性能优化示例
    /// </summary>
    public static async Task RunAllExamples()
    {
        Console.WriteLine("=== 使用自定义 HttpClient 示例 ===");
        await UseCustomHttpClientExample();

        Console.WriteLine("\n=== 并行请求示例 ===");
        await UseParallelRequestsExample();
    }
}
