namespace Raven.Live.Services;

public interface IRavenUsersClient
{
    Task ReportUserOnSite(string idCard);
    Task<string?> GetVersion();
}
