using RestSharp;
using RestSharp.Authenticators;
using Microsoft.Extensions.Caching.Memory;

namespace Raven.Live;

public class ClientCredentialsAuthenticator : AuthenticatorBase
{
    private readonly string _authentikUrl;
    private readonly string _clientId;
    private readonly string _clientUsername;
    private readonly string _clientPassword;
    private readonly MemoryCache _tokenCache = new(new MemoryCacheOptions());

    public ClientCredentialsAuthenticator(string authentikUrl, string clientId, string clientUsername, string clientPassword) : base("")
    {
        _authentikUrl = authentikUrl;
        _clientId = clientId;
        _clientUsername = clientUsername;
        _clientPassword = clientPassword;
    }

    /// <summary>
    /// Called by the REST library when it needs to authenticate a request
    /// </summary>
    /// <inheritdoc cref="GetAuthenticationParameter"/>
    protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
    {
        var token = _tokenCache.Get<string>(_clientId) ?? await GetToken();
        return new HeaderParameter(KnownHeaders.Authorization, token);
    }

    /// <summary>
    /// Request a token from Authentik
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetToken()
    {
        var options = new RestClientOptions(_authentikUrl);
        using var client = new RestClient(options);

        var request = new RestRequest("application/o/token/")
            .AddParameter("grant_type", "client_credentials")
            .AddParameter("client_id", _clientId)
            .AddParameter("username", _clientUsername)
            .AddParameter("password", _clientPassword)
            .AddParameter("scope", "raven:live");
        var response = await client.PostAsync<TokenResponse>(request);

        var token = $"{response!.TokenType} {response.AccessToken}";

        _tokenCache.CreateEntry(_clientId)
            .SetValue(token)
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(response.ExpiresIn - 10));

        return token;
    }
}
