using Raven.Live;
using Raven.Live.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(serviceProvider => 
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new ClientCredentialsAuthenticator(
        config["users:Authority"],
        config["users:ClientId"],
        config["users:ClientUsername"],
        config["users:ClientPassword"]
        );
});
builder.Services.AddSingleton<IRavenUsersClient, RavenUsersClient>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new RavenUsersClient(config["raven:users"],
        serviceProvider.GetRequiredService<ClientCredentialsAuthenticator>());
});
builder.Services.AddHostedService<MQTTListener>();

var app = builder.Build();

app.UseHealthChecks("/healthz");

await app.RunAsync();

