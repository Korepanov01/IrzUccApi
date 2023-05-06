using IrzUccApi.Models.Configurations;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace IrzUccApi.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtConfiguration jwtConfiguration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecurityKey)),

                    ValidateIssuer = false,
                    ValidIssuer = jwtConfiguration.Issuer,

                    ValidateAudience = false,
                    ValidAudience = jwtConfiguration.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerGenExt(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        jwtSecurityScheme,
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsExt(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            return services;
        }

        public static IServiceCollection ConfigureIdentityOptions(
            this IServiceCollection services,
            PasswordConfiguration passwordConfiguration)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = passwordConfiguration.RequireDigit;
                options.Password.RequireLowercase = passwordConfiguration.RequireLowercase;
                options.Password.RequireNonAlphanumeric = passwordConfiguration.RequireNonAlphanumeric;
                options.Password.RequireUppercase = passwordConfiguration.RequireUppercase;
                options.Password.RequiredLength = passwordConfiguration.RequiredLength;
                options.Password.RequiredUniqueChars = passwordConfiguration.RequiredUniqueChars;

                options.User.RequireUniqueEmail = true;
            });

            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<EventsNotifier>();
            services.AddHostedService<MemoryСleaner>();

            return services;
        }
    }
}
