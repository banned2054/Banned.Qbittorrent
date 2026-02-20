using Banned.Qbittorrent.Exceptions;

namespace Banned.Qbittorrent.Services;

/// <summary>
/// 提供与 qBittorrent 身份验证相关的服务<br/>
/// Provides services related to qBittorrent authentication
/// </summary>
public class AuthenticationService : IDisposable
{
    private readonly NetService     _netService;
    private readonly string         _userName;
    private readonly string         _password;
    private readonly SemaphoreSlim  _loginLock = new(1, 1);
    private volatile bool           _isLoggedIn;
    private          DateTimeOffset _loginExpiry = DateTimeOffset.MinValue;

    /// <summary>
    /// 初始化 <see cref="AuthenticationService"/> 类的新实例。<br/>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="netService">网络服务实例。 / Network service instance.</param>
    /// <param name="userName">用户名。 / Username.</param>
    /// <param name="password">密码。 / Password.</param>
    public AuthenticationService(NetService netService, string userName, string password)
    {
        _netService = netService;
        _userName   = userName;
        _password   = password;

        _netService.EnsureLoggedInHandler = EnsureLoggedIn;
    }

    /// <summary>
    /// 显式登录 qBittorrent。<br/>
    /// Explicitly log in to qBittorrent.
    /// </summary>
    /// <remarks>
    /// 此方法会强制触发登录流程，无论当前是否已登录。<br/>
    /// This method forces the login process regardless of the current login status.
    /// </remarks>
    public async Task Login()
    {
        await EnsureLoggedIn(force : true);
    }

    /// <summary>
    /// 登出 qBittorrent 客户端。<br/>
    /// Log out of the qBittorrent client.
    /// </summary>
    /// <remarks>
    /// 登出后，本地缓存的登录状态和过期时间将被重置。<br/>
    /// After logging out, the locally cached login status and expiration time will be reset.
    /// </remarks>
    public async Task Logout()
    {
        if (!_isLoggedIn) return;

        try
        {
            await _netService.Post("api/v2/auth/logout", skipAuthCheck : true);
        }
        finally
        {
            _isLoggedIn  = false;
            _loginExpiry = DateTimeOffset.MinValue;
        }
    }

    /// <summary>
    /// 供 NetService 调用的保活检查方法，确保当前处于登录状态。<br/>
    /// Keep-alive check method called by NetService to ensure the current session is logged in.
    /// </summary>
    private async Task EnsureLoggedIn() => await EnsureLoggedIn(false);

    /// <summary>
    /// 确保用户已登录，并在必要时执行登录操作。<br/>
    /// Ensures the user is logged in and performs the login operation if necessary.
    /// </summary>
    /// <param name="force">是否强制重新登录。 / Whether to force re-login.</param>
    /// <exception cref="QbittorrentLoginFailedException">登录响应不为 "Ok." 时抛出。 / Thrown when the login response is not "Ok.".</exception>
    private async Task EnsureLoggedIn(bool force)
    {
        if (!force && _isLoggedIn && DateTimeOffset.UtcNow < _loginExpiry) return;

        await _loginLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!force && _isLoggedIn && DateTimeOffset.UtcNow < _loginExpiry) return;

            var parameters = new Dictionary<string, string>
            {
                { "username", _userName },
                { "password", _password }
            };

            var response = await _netService.Post("api/v2/auth/login", parameters, skipAuthCheck : true);

            if (!response.Contains("Ok.", StringComparison.OrdinalIgnoreCase))
            {
                _isLoggedIn = false;
                throw new QbittorrentLoginFailedException("Login response was not 'Ok.'", 200);
            }

            _isLoggedIn  = true;
            _loginExpiry = DateTimeOffset.UtcNow.AddHours(1);
        }
        catch (QbittorrentException)
        {
            _isLoggedIn = false;
            throw;
        }
        finally
        {
            _loginLock.Release();
        }
    }

    /// <summary>
    /// 释放 <see cref="AuthenticationService"/> 使用的资源。<br/>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _loginLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
