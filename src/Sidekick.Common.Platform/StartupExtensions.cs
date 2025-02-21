using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Keyboards;
using Sidekick.Common.Platform.Localization;
using Sidekick.Common.Platform.Windows.Processes;

namespace Sidekick.Common.Platform;

/// <summary>
/// Functions for startup configuration for platform related features
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds platform (operating system) functions to the service collection.
    /// </summary>
    /// <param name="services">The services collection to add services to.</param>
    /// <param name="options">The platform options.</param>
    /// <returns>The service collection with services added.</returns>
    public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services, Action<PlatformOptions> options)
    {
        services.Configure(options);

        services.AddTransient<PlatformResources>();
        services.AddTransient<IClipboardProvider, ClipboardProvider>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSidekickInitializableService<IProcessProvider, ProcessProvider>();
        }

        services.AddSidekickInitializableService<IKeyboardProvider, KeyboardProvider>();
        services.AddSingleton<IGameLogProvider, GameLogProvider>();

        foreach (var keybind in SidekickConfiguration.Keybinds)
        {
            SidekickConfiguration.InitializableServices.Add(keybind);
            services.AddSingleton(keybind);
        }

        return services;
    }
}
