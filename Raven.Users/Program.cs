using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Raven.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.Audience = builder.Configuration["oidc:ClientId"];
        jwtOptions.Authority = builder.Configuration["oidc:Authority"];
    })
    /*.AddOpenIdConnect(oidcOptions =>
    {
        oidcOptions.ClientId = builder.Configuration["oidc:ClientId"];
        oidcOptions.ClientSecret = builder.Configuration["oidc:clientsecret"];
        oidcOptions.MetadataAddress = builder.Configuration["oidc:Authority"];

        oidcOptions.ResponseType = "code";
        oidcOptions.GetClaimsFromUserInfoEndpoint = true;
       
    }
)*/;

builder.Services.AddAuthorization(options =>
{
    var scopes = new[] {
        "raven:live"
    };

    Array.ForEach(scopes, scope =>
        options.AddPolicy(scope,
            policy => policy.Requirements.Add(
                new ScopeRequirement(builder.Configuration["oidc:Authority"], scope)
            )
        )
    );
});

builder.Services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();

builder.Services.AddControllers();
builder.Services.AddSilverback()
    .UseModel()
    .WithConnectionToMessageBroker(brokerOptions => brokerOptions.AddMqtt())
    .AddEndpointsConfigurator<EndpointsConfigurator>()
    .AddSingletonSubscriber<ClockUserSubscriber>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/healthz");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
