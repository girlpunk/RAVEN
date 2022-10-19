using RestSharp;

namespace Raven.Live.Services;

public class RavenUsersClient : IRavenUsersClient, IDisposable
{
    private readonly RestClient _client;

    public RavenUsersClient(string ravenLiveUrl, ClientCredentialsAuthenticator authenticator)
    {
        var options = new RestClientOptions(ravenLiveUrl)
        {
            FailOnDeserializationError = true,
            ThrowOnAnyError = true
        };

#if DEBUG
        // Don't validate SSL certificates in debug mode
        options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
#endif

        _client = new RestClient(options)
        {
            Authenticator = authenticator
        };
    }

    public async Task<string?> GetVersion()
    {
        var response = await _client.GetAsync(new RestRequest("/Version"));
        return response.Content;
    }

    public async Task ReportUserOnSite(string user) =>
        await _client.PostJsonAsync(
            "users/reportOnSite/",
            new
            {
                id_card = user
            }
        );

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
