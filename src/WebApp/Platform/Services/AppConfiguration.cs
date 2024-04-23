using MyApp.WebApp.Platform.Settings;

namespace MyApp.WebApp.Platform.Services;

public static class AppConfiguration
{
    public static void BindSettings(this WebApplicationBuilder builder)
    {
        builder.Configuration.GetSection(nameof(ApplicationSettings.RaygunSettings))
            .Bind(ApplicationSettings.RaygunSettings);

        var useDevConfig = Convert.ToBoolean(builder.Configuration["UseDevSettings"]);
        var devConfig = builder.Configuration.GetSection(nameof(ApplicationSettings.DevSettings));

        if (useDevConfig && devConfig.Exists())
            devConfig.Bind(ApplicationSettings.DevSettings);
        else
            ApplicationSettings.DevSettings = ApplicationSettings.ProductionDefault;
    }
}
