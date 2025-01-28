using MyApp.WebApp.Platform.Settings;
using System.Reflection;

namespace MyApp.WebApp.Platform.AppConfiguration;

public static class BindingsConfiguration
{
    public static void BindSettings(WebApplicationBuilder builder)
    {
        // Bind app settings.
        builder.Configuration.GetSection(nameof(AppSettings.SupportSettings))
            .Bind(AppSettings.SupportSettings);

        builder.Configuration.GetSection(nameof(AppSettings.RaygunSettings))
            .Bind(AppSettings.RaygunSettings);

        // Set app version.
        var entryAssembly = Assembly.GetEntryAssembly();
        var segments = (entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? entryAssembly?.GetName().Version?.ToString() ?? "").Split('+');

        AppSettings.SupportSettings.InformationalVersion = segments[0];
        if (segments.Length > 0)
            AppSettings.SupportSettings.InformationalBuild = segments[1][..Math.Min(7, segments[1].Length)];

        // Dev settings should only be used in development environment and when explicitly enabled.
        var devConfig = builder.Configuration.GetSection(nameof(AppSettings.DevSettings));
        var useDevConfig = builder.Environment.IsDevelopment() && devConfig.Exists() &&
                           Convert.ToBoolean(devConfig[nameof(AppSettings.DevSettings.UseDevSettings)]);

        if (useDevConfig)
            devConfig.Bind(AppSettings.DevSettings);
        else
            AppSettings.DevSettings = AppSettings.ProductionDefault;
    }
}
