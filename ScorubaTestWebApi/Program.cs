using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilitar logs detallados para depuración
IdentityModelEventSource.ShowPII = true;

var signingKey = new JsonWebKey(@"{ 
    ""kty"": ""RSA"",
    ""kid"": ""6D8F84CEC4DA5D68351358FB4BE035AB"",
    ""use"": ""sig"",
    ""alg"": ""RS256"",
    ""n"": ""3r7QXZJAARDWyUqjwITT4SPa9uRclwLRJF11gXooqQAksOsqEZSLUR1Vf6gXzXZDMSq5i3jTZxJvQG6qtKlVqScdANw3lJpHLxrRdy3METd2QyVy61uspP8aBMYwCBAVufcBAIDQ6pfVixXYjh_OACAaSET_R0pfnajbvAtdi8f9hbchU4RH6sT5gAhPNJXcPA2zsUJOuUNg9VSDB1d1pI7uHWnAGV-awOg58iAaMga8iRDswlzcuZRcuqGrxmUBI2Q4mqp3hPKIgf6BIC65vu-48yoQ7jjPt4v6zddUhcqEl3HxohrHo6iHDPoXJ3VNGUztjCjfMfIZgGlJSn1VaQ"",
    ""e"": ""AQAB""
}");

// Add Auth JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44310/"; // URL de IdentityServer
        options.Audience = "skoruba_test_api"; // El scope configurado para la API
        options.RequireHttpsMetadata = false;

        //Forzar la obtención de claves de IdentityServer
        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://localhost:44310/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever()
        );

        // Forzar la obtención de claves de firma
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "https://localhost:44310/",
            ValidAudience = "skoruba_test_api",
            IssuerSigningKey = signingKey
        };



        options.Events = new JwtBearerEvents
        {

            OnAuthenticationFailed = context =>
            {

                Console.WriteLine($"🔴 Error de autenticación: {context.Exception.Message}");

                // Imprimir claves de firma disponibles
                var parameters = context.Options.TokenValidationParameters;
                if (parameters.IssuerSigningKey != null)
                {
                    Console.WriteLine($"IssuerSigningKey: {parameters.IssuerSigningKey}");
                }
                else
                {
                    Console.WriteLine("No se encontraron claves de firma.");
                }

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("🔴 Desafío de autenticación fallido.");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("🟢 Token validado correctamente.");
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "skoruba_test_api.read"));
        options.AddPolicy("CanWrite", policy => policy.RequireClaim("scope", "skoruba_test_api.write"));
    });



builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
