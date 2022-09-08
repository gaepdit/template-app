using Microsoft.AspNetCore.DataProtection;
using MyAppRoot.AppServices.ServiceCollectionExtensions;
using MyAppRoot.WebApp.Platform.Local;
using MyAppRoot.WebApp.Platform.Services;
using MyAppRoot.WebApp.Platform.Settings;

var builder = WebApplication.CreateBuilder(args);
var isLocal = builder.Environment.IsLocalEnv();

// Bind application settings.
builder.Configuration.GetSection(nameof(ApplicationSettings.LocalDevSettings))
    .Bind(ApplicationSettings.LocalDevSettings);

// Configure Identity.
builder.Services.AddIdentityStores(isLocal);

// Configure cookies (SameSiteMode.None is needed to get single sign-out to work).
builder.Services.Configure<CookiePolicyOptions>(opts => opts.MinimumSameSitePolicy = SameSiteMode.None);

// Configure Authentication.
builder.Services.AddAuthenticationServices(builder.Configuration, isLocal);

// Persist data protection keys.
var keysFolder = Path.Combine(builder.Configuration["PersistedFilesBasePath"], "DataProtectionKeys");
builder.Services.AddDataProtection().PersistKeysToFileSystem(Directory.CreateDirectory(keysFolder));
builder.Services.AddAuthorization();

// Configure UI services.
builder.Services.AddRazorPages();
// Starting value for HSTS max age is five minutes to allow for debugging.
// For more info on updating HSTS max age value for production, see:
// https://gaepdit.github.io/web-apps/use-https.html#how-to-enable-hsts
if (!isLocal) builder.Services.AddHsts(opts => opts.MaxAge = TimeSpan.FromMinutes(300));

// Add App and data services.
builder.Services.AddAppServices();
builder.Services.AddDataServices(builder.Configuration, isLocal);

// Initialize database
builder.Services.AddHostedService<MigratorHostedService>();

// Build the application
var app = builder.Build();
var env = app.Environment;

// Configure the HTTP request pipeline.
if (env.IsDevelopment() || env.IsLocalEnv())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Production or Staging
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure the application
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints.
app.MapRazorPages();
// MapControllers is only needed if an API is implemented.
// app.MapControllers();

// Make it so.
app.Run();
