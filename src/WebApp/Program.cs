using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using Mindscape.Raygun4Net.AspNetCore;
using MyApp.AppServices.RegisterServices;
using MyApp.WebApp.Platform.Raygun;
using MyApp.WebApp.Platform.SecurityHeaders;
using MyApp.WebApp.Platform.Services;
using MyApp.WebApp.Platform.Settings;

var builder = WebApplication.CreateBuilder(args);

// Set default timeout for regular expressions.
// https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices#use-time-out-values
// ReSharper disable once HeapView.BoxingAllocation
AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

// Bind application settings.
AppConfiguration.BindSettings(builder);

// Configure Identity.
builder.Services.AddIdentityStores();

// Configure Authentication.
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformation>();

// Persist data protection keys.
var keysFolder = Path.Combine(builder.Configuration["PersistedFilesBasePath"] ?? "", "DataProtectionKeys");
builder.Services.AddDataProtection().PersistKeysToFileSystem(Directory.CreateDirectory(keysFolder));

// Configure authorization policies.
builder.Services.AddAuthorizationPolicies();

// Configure UI services.
builder.Services.AddRazorPages();

// Starting value for HSTS max age is five minutes to allow for debugging.
// For more info on updating HSTS max age value for production, see:
// https://gaepdit.github.io/web-apps/use-https.html#how-to-enable-hsts
if (!builder.Environment.IsDevelopment())
    builder.Services.AddHsts(opts => opts.MaxAge = TimeSpan.FromMinutes(300));

// Configure application monitoring.
if (!string.IsNullOrEmpty(ApplicationSettings.RaygunSettings.ApiKey))
{
    builder.Services.AddTransient<IErrorLogger, ErrorLogger>();
    builder.Services.AddRaygun(builder.Configuration,
        new RaygunMiddlewareSettings { ClientProvider = new RaygunClientProvider() });
    builder.Services.AddHttpContextAccessor(); // needed by RaygunScriptPartial
}

// Add app services.
builder.Services.AddAutoMapperProfiles();
builder.Services.AddAppServices();
builder.Services.AddValidators();

// Add data stores.
builder.Services.AddDataStores(builder.Configuration);

// Initialize database.
builder.Services.AddHostedService<MigratorHostedService>();

// Add API documentation.
builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MY_APP API",
        Contact = new OpenApiContact
        {
            Name = "MY_APP Support",
            Email = builder.Configuration["SupportEmail"],
        },
    });
});

// Configure bundling and minification.
builder.Services.AddWebOptimizer();

//Add simple cache.
builder.Services.AddMemoryCache();

// Build the application.
var app = builder.Build();

// Configure error handling.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage(); // Development
else app.UseExceptionHandler("/Error"); // Production or Staging

// Configure security HTTP headers
if (!app.Environment.IsDevelopment() || ApplicationSettings.DevSettings.UseSecurityHeadersInDev)
{
    app.UseHsts();
    app.UseSecurityHeaders(policyCollection => policyCollection.AddSecurityHeaderPolicies());
}

if (!string.IsNullOrEmpty(ApplicationSettings.RaygunSettings.ApiKey)) app.UseRaygun();

// Configure the application pipeline.
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure API documentation.
app.UseSwagger(c => { c.RouteTemplate = "api-docs/{documentName}/openapi.json"; });
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/openapi.json", "MY_APP API v1");
    c.RoutePrefix = "api-docs";
    c.DocumentTitle = "MY_APP_NAME API";
});

// Map endpoints.
app.MapRazorPages();
app.MapControllers();

// Make it so.
app.Run();
