using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.WebAPI.Authentication.Service;
using ProjectManager.WebAPI.Authentication.Utils;
using ProjectManager.WebAPI.Authentication.Validators;
using System.Text;

namespace ProjectManager.WebAPI.Authentication;

public static class Configuration
{
    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        JwtSettings settings = new();
        configuration.Bind(JwtSettings.SectionName, settings);

        services.AddSingleton(Options.Create(settings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IValidator<LoginDTO>, LoginDTOValidator>();
        services.AddScoped<IValidator<RegisterDTO>, RegisterDTOValidator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = settings.Audience,
                        ValidIssuer = settings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
                    };
                });

        return services;
    }
}
