using Banned.Qbittorrent.Exceptions;

namespace Banned.Qbittorrent.Services;

public class AuthenticationService : IDisposable
{
    private readonly NetService     _netService;
    private readonly string         _userName;
    private readonly string         _password;
    private readonly SemaphoreSlim  _loginLock = new(1, 1);
    private volatile bool           _isLoggedIn;
    private          DateTimeOffset _loginExpiry = DateTimeOffset.MinValue;

    public AuthenticationService(NetService netService, string userName, string password)
    {
        _netService = netService;
        _userName   = userName;
        _password   = password;

        _netService.EnsureLoggedInHandler = EnsureLoggedIn;
    }

    /// <summary>
    /// 显式登录
    /// </summary>
    public async Task Login()
    {
        await EnsureLoggedIn(force : true);
    }

    /// <summary>
    /// 登出
    /// </summary>
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
    /// 供 NetService 调用的保活检查方法，也供内部使用
    /// </summary>
    private async Task EnsureLoggedIn() => await EnsureLoggedIn(false);

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

    public void Dispose()
    {
        _loginLock.Dispose();
    }
}
