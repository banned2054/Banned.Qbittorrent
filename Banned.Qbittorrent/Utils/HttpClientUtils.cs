using Banned.Qbittorrent.Models.Enums;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Banned.Qbittorrent.Utils;

/// <summary>
/// 提供默认 HTTP 客户端及地址族连接相关的无状态方法。<br/>
/// Provides stateless helpers for default HTTP clients and address-family connections.
/// </summary>
internal static class HttpClientUtils
{
    internal static HttpClient CreateDefaultHttpClient(
        CookieContainer                                    cookieContainer,
        TimeSpan                                           timeout,
        TimeSpan                                           connectTimeout,
        AddressFamilyPreference                            preference,
        Func<string, CancellationToken, Task<IPAddress[]>> addressResolver,
        Action<string>?                                    diagnosticLogger)
    {
        var handler = new SocketsHttpHandler
        {
            CookieContainer             = cookieContainer,
            AllowAutoRedirect           = true,
            UseCookies                  = true,
            UseProxy                    = false,
            ConnectTimeout              = connectTimeout,
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
            PooledConnectionLifetime    = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer     = 16,
            AutomaticDecompression = DecompressionMethods.GZip    |
                                     DecompressionMethods.Deflate |
                                     DecompressionMethods.Brotli
        };

        if (preference != AddressFamilyPreference.System)
        {
            handler.ConnectCallback = (context, cancellationToken) => ConnectAsync(
             context.DnsEndPoint,
             preference,
             addressResolver,
             ConnectSocketAsync,
             diagnosticLogger,
             cancellationToken);
        }

        var client = new HttpClient(handler) { Timeout = timeout };
        client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true
        };
        return client;
    }

    internal static Task<IPAddress[]> ResolveHostAddressesAsync(
        string            host,
        CancellationToken cancellationToken) =>
        Dns.GetHostAddressesAsync(host, cancellationToken);

    internal static async ValueTask<Stream> ConnectAsync(
        DnsEndPoint                                                endpoint,
        AddressFamilyPreference                                    preference,
        Func<string, CancellationToken, Task<IPAddress[]>>         addressResolver,
        Func<IPAddress, int, CancellationToken, ValueTask<Stream>> socketConnector,
        Action<string>?                                            diagnosticLogger,
        CancellationToken                                          cancellationToken)
    {
        var addresses  = await addressResolver(endpoint.Host, cancellationToken).ConfigureAwait(false);
        var candidates = OrderAddresses(addresses, preference).ToArray();
        if (candidates.Length == 0)
            throw new SocketException((int)SocketError.HostNotFound);

        Exception? lastException = null;
        foreach (var address in candidates)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var stream = await socketConnector(address, endpoint.Port, cancellationToken).ConfigureAwait(false);
                diagnosticLogger?.Invoke(
                                         $"ConnectCallback selected family={address.AddressFamily} address={address} elapsed={stopwatch.Elapsed}");
                return stream;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (exception is SocketException or IOException)
            {
                lastException = exception;
                diagnosticLogger?.Invoke(
                                         $"ConnectCallback failed family={address.AddressFamily} address={address} elapsed={stopwatch.Elapsed} " +
                                         $"exception={exception.GetType().Name}: {exception.Message}");
            }
        }

        throw lastException ?? new SocketException((int)SocketError.NotConnected);
    }

    internal static IEnumerable<IPAddress> OrderAddresses(
        IEnumerable<IPAddress>  addresses,
        AddressFamilyPreference preference)
    {
        var supported = addresses.Where(address =>
                                            address.AddressFamily is AddressFamily.InterNetwork
                                                                  or AddressFamily.InterNetworkV6);
        var preferredFamily = preference switch
        {
            AddressFamilyPreference.PreferIPv4 => AddressFamily.InterNetwork,
            AddressFamilyPreference.PreferIPv6 => AddressFamily.InterNetworkV6,
            _                                  => AddressFamily.Unspecified
        };

        return preferredFamily == AddressFamily.Unspecified
            ? supported
            : supported.OrderBy(address => address.AddressFamily == preferredFamily ? 0 : 1);
    }

    private static async ValueTask<Stream> ConnectSocketAsync(
        IPAddress         address,
        int               port,
        CancellationToken cancellationToken)
    {
        var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        try
        {
            await socket.ConnectAsync(new IPEndPoint(address, port), cancellationToken).ConfigureAwait(false);
            return new NetworkStream(socket, ownsSocket : true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
}
