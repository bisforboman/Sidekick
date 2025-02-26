using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Platform.Clipboard;
using Sidekick.Common.Platform.GameLogs;
using Sidekick.Common.Platform.Keyboards;
using Sidekick.Common.Platform.Localization;
using Sidekick.Common.Platform.Windows;
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
    public static IServiceCollection AddSidekickCommonPlatform(this IServiceCollection services) =>
        services
            .AddSingleton<PlatformResources>()
            .AddSingleton<IClipboardProvider, ClipboardProvider>()
            .AddSingleton<IGameLogProvider, GameLogProvider>();

    public static IServiceCollection AddWindowsSpecificServices(this IServiceCollection services) =>
        services
            .AddSidekickInitializableService<IProcessProvider, ProcessProvider>()
            .AddSidekickInitializableService<IKeyboardProvider, KeyboardProvider>();
}
