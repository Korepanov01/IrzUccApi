using IrzUccApi.Db;
using IrzUccApi.Models.Configurations;
using IrzUccApi.Models.Db;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtConfiguration = new JwtConfiguration();
builder.Configuration.Bind("Jwt", jwtConfiguration);
builder.Services.AddSingleton(jwtConfiguration);

builder.Services.AddAuthentication(options =>
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
builder.Services.AddAuthorization();

builder.Services.AddTransient<JwtService>();

var emailConfiguration = new EmailConfiguration();
builder.Configuration.Bind("EmailService", emailConfiguration);
builder.Services.AddSingleton(emailConfiguration);
builder.Services.AddTransient<EmailService>();

var passwordConfiguration = new PasswordConfiguration();
builder.Configuration.Bind("Password", passwordConfiguration);
builder.Services.AddSingleton(passwordConfiguration);
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = passwordConfiguration.RequireDigit;
    options.Password.RequireLowercase = passwordConfiguration.RequireLowercase;
    options.Password.RequireNonAlphanumeric = passwordConfiguration.RequireNonAlphanumeric;
    options.Password.RequireUppercase = passwordConfiguration.RequireUppercase;
    options.Password.RequiredLength = passwordConfiguration.RequiredLength;
    options.Password.RequiredUniqueChars = passwordConfiguration.RequiredUniqueChars;

    options.User.RequireUniqueEmail = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
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

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
