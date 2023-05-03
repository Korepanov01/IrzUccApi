using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Extensions;
using IrzUccApi.Hubs;
using IrzUccApi.Models.Configurations;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<AppIdentityErrorDescriber>();

var jwtConfiguration = new JwtConfiguration();
builder.Configuration.Bind("Jwt", jwtConfiguration);
builder.Services.AddSingleton(jwtConfiguration);
builder.Services.AddJwtAuthentication(jwtConfiguration);
builder.Services.AddAuthorization();
builder.Services.AddTransient<JwtService>();

var backgroundServicesConfiguration = new MemoryCleanerConfiguration();
builder.Configuration.Bind("MemoryCleanerConfiguration", backgroundServicesConfiguration);
builder.Services.AddSingleton(backgroundServicesConfiguration);
builder.Services.AddBackgroundServices();

builder.Services.AddScoped<UnitOfWork>();

var emailConfiguration = new EmailConfiguration();
builder.Configuration.Bind("EmailService", emailConfiguration);
builder.Services.AddSingleton(emailConfiguration);
builder.Services.AddTransient<EmailService>();

var passwordConfiguration = new PasswordConfiguration();
builder.Configuration.Bind("Password", passwordConfiguration);
builder.Services.AddSingleton(passwordConfiguration);
builder.Services.ConfigureIdentityOptions(passwordConfiguration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenExt();

builder.Services.AddCorsExt();

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed((host) => true)
    .AllowCredentials();
});

app.UseAuthentication();

app.MapControllers();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/hubs/chat");
});

app.Run();
