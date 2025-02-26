using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Cache;
using Sidekick.Common.Database;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1;

public class ParserFixture : IAsyncLifetime
{
    private static Task? initializationTask;

    public IInvariantModifierProvider InvariantModifierProvider { get; private set; } = null!;

    public IItemParser Parser { get; private set; } = null!;
    private TestContext TestContext { get; set; } = null!;

    public Task DisposeAsync()
    {
        TestContext.Dispose();
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        TestContext = new TestContext();
        TestContext.Services.AddLocalization();

        TestContext.Services
            // Building blocks
            .AddSidekickCommon()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)

                // Apis
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoeWikiApi();

        var settingsService = TestContext.Services.GetRequiredService<ISettingsService>();
        await settingsService.Set(SettingKeys.LanguageParser, "en");
        await settingsService.Set(SettingKeys.LanguageUi, "en");
        await settingsService.Set(SettingKeys.LeagueId, "poe1.Standard");

        initializationTask ??= Initialize(TestContext.Services);

        await initializationTask;

        Parser = TestContext.Services.GetRequiredService<IItemParser>();
        InvariantModifierProvider = TestContext.Services.GetRequiredService<IInvariantModifierProvider>();
    }

    private static async Task Initialize(IServiceProvider serviceProvider)
    {
        var cache = serviceProvider.GetRequiredService<ICacheProvider>();
        await cache.Clear();

        var logger = serviceProvider.GetRequiredService<ILogger<ParserFixture>>();
        foreach (var initializableService in serviceProvider.GetServices<IInitializableService>())
        {
            logger.LogInformation($"[Initialization] Initializing {initializableService.GetType().FullName}");
            await initializableService.Initialize();
        }
    }
}
