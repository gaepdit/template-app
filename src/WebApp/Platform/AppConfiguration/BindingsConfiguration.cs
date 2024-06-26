﻿using MyApp.WebApp.Platform.Settings;

namespace MyApp.WebApp.Platform.AppConfiguration;

public static class BindingsConfiguration
{
    public static void BindSettings(WebApplicationBuilder builder)
    {
        // App settings
        builder.Configuration.GetSection(nameof(AppSettings.SupportSettings))
            .Bind(AppSettings.SupportSettings);
        builder.Configuration.GetSection(nameof(AppSettings.RaygunSettings))
            .Bind(AppSettings.RaygunSettings);

        // Dev settings
        var devConfig = builder.Configuration.GetSection(nameof(AppSettings.DevSettings));
        var useDevConfig = devConfig.Exists() &&
            Convert.ToBoolean(devConfig[nameof(AppSettings.DevSettings.UseDevSettings)]);
        if (useDevConfig)
            devConfig.Bind(AppSettings.DevSettings);
        else
            AppSettings.DevSettings = AppSettings.ProductionDefault;
    }
}
