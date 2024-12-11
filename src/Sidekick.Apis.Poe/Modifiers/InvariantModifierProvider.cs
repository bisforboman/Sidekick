using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Modifiers;

public class InvariantModifierProvider
(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IInvariantModifierProvider
{
    public List<string> IncursionRoomModifierIds { get; } = new();

    public List<string> LogbookFactionModifierIds { get; } = new();

    public string ClusterJewelSmallPassiveCountModifierId { get; private set; } = null!;

    public string ClusterJewelSmallPassiveGrantModifierId { get; private set; } = null!;

    public Dictionary<int, string> ClusterJewelSmallPassiveGrantOptions { get; private set; } = null!;

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var result = await GetList();
        InitializeIncursionRooms(result);
        InitializeLogbookFactions(result);
        InitializeClusterJewel(result);
    }

    private void InitializeIncursionRooms(List<ApiCategory> apiCategories)
    {
        IncursionRoomModifierIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            var first = apiCategory.Entries.FirstOrDefault();
            if (first
                    ?.Id.Split('.')[0]
                != "pseudo")
            {
                return;
            }

            IncursionRoomModifierIds.AddRange(apiCategory
                                                  .Entries.Where(x => x.Text.StartsWith("Has Room: "))
                                                  .Select(x => x.Id)
                                                  .ToList());
        }
    }

    private void InitializeLogbookFactions(List<ApiCategory> apiCategories)
    {
        LogbookFactionModifierIds.Clear();
        foreach (var apiCategory in apiCategories)
        {
            var first = apiCategory.Entries.FirstOrDefault();
            if (first
                    ?.Id.Split('.')[0]
                != "pseudo")
            {
                return;
            }

            LogbookFactionModifierIds.AddRange(apiCategory
                                                   .Entries.Where(x => x.Text.StartsWith("Has Logbook Faction: "))
                                                   .Select(x => x.Id)
                                                   .ToList());
        }
    }

    private void InitializeClusterJewel(List<ApiCategory> apiCategories)
    {
        foreach (var apiCategory in apiCategories)
        {
            var first = apiCategory.Entries.FirstOrDefault();
            if (first
                    ?.Id.Split('.')[0]
                != "enchant")
            {
                continue;
            }

            foreach (var apiModifier in apiCategory.Entries)
            {
                if (apiModifier.Text == "Adds # Passive Skills")
                {
                    ClusterJewelSmallPassiveCountModifierId = apiModifier.Id;
                }

                if (apiModifier.Text != "Added Small Passive Skills grant: #")
                {
                    continue;
                }

                ClusterJewelSmallPassiveGrantModifierId = apiModifier.Id;
                if (apiModifier.Option == null)
                {
                    return;
                }

                ClusterJewelSmallPassiveGrantOptions = apiModifier.Option.Options.ToDictionary(x => x.Id, x => x.Text!);
            }
        }
    }

    public async Task<List<ApiCategory>> GetList()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantModifiers";

        return await cacheProvider.GetOrSet(cacheKey,
                                            async () =>
                                            {
                                                var result = await poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "data/stats");
                                                return result.Result;
                                            });
    }
}
