using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Modules.Wealth;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickWealth(this IServiceCollection services, bool withKeybindHandler = true)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSingleton<WealthParser>();

        if (withKeybindHandler)
        {
            services.AddSidekickKeybind<OpenWealthKeybindHandler>();
        }

        return services;
    }
}
