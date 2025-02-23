using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Trade.Keybinds;
using Sidekick.Modules.Trade.Localization;

namespace Sidekick.Modules.Trade;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickTrade(this IServiceCollection services, bool withKeybindHandler = true)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddTransient<TradeResources>();
        services.AddScoped<PriceCheckService>();

        if (withKeybindHandler)
        {
            services.AddSidekickKeybind<PriceCheckItemKeybindHandler>();
        }

        return services;
    }
}
