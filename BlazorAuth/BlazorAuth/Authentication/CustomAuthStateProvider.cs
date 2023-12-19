using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace BlazorAuth.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider

    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;

        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
{
    try
    {
        var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
        var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

        if (userSession == null)
            return await Task.FromResult(new AuthenticationState(_anonymous));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userSession.UserName)
        };
        claims.AddRange(userSession.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }
    catch
    {
        return await Task.FromResult(new AuthenticationState(_anonymous));
    }
}

public async Task UpdateAuthenticationState(UserSession userSession)
{
    ClaimsPrincipal claimsPrincipal;

    if (userSession != null)
    {
        await _sessionStorage.SetAsync("UserSession", userSession);
        var claims = new List<Claim> 
        {
            new Claim(ClaimTypes.Name, userSession.UserName)
        };
        claims.AddRange(userSession.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
    }
    else
    {
        await _sessionStorage.DeleteAsync("UserSession");
        claimsPrincipal = _anonymous;
    }
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
}


    }
}

