using Mental_Notes.Components;
using Microsoft.EntityFrameworkCore;
using Resend;
using Services;
using SharedData.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var configuration = builder.Configuration;
// On Azure (afterpublish) the Default connection string in Azure overrides the appsettings.json
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(item =>
    item.UseSqlServer(connectionString,
    b => {
        b.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        b.MigrationsAssembly("Mental-Notes");
    }));

builder.Services.Configure<ResendClientOptions>(o =>
{
    var apiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN");
    // Do not log the actual token value
    if (string.IsNullOrEmpty(apiToken))
    {
        Console.WriteLine("Resend API token is not set.");
    }
    o.ApiToken = apiToken!;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<ResendClient>();
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddSingleton<RateLimiter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
