using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Static.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Static;

public class ItemStaticDataProvider
(
    ICacheProvider cacheProvider,
    IPoeTradeClient poeTradeClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : IItemStaticDataProvider
{
    private Dictionary<string, StaticItem> ByIds { get; } = new();

    private Dictionary<string, StaticItem> ByTexts { get; } = new();

    private Task? initialization;
    /// <inheritdoc/>
    public Task Initialization => initialization ??= Initialize();

    private async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_StaticData";
        var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<StaticItemCategory>(game, gameLanguageProvider.Language, "data/static"), (cache) => cache.Result.Any());

        ByTexts.Clear();
        ByIds.Clear();
        foreach (var category in result.Result)
        {
            foreach (var entry in category.Entries)
            {
                if (entry.Id == null! || entry.Image == null || entry.Text == null)
                {
                    continue;
                }

                ByIds.Add(entry.Id, entry);
                ByTexts.TryAdd(entry.Text, entry);
            }
        }
    }

    public string? GetImage(string id)
    {
        var result = Get(id);
        if (result?.Image == null) return null;

        return $"{gameLanguageProvider.Language.PoeCdnBaseUrl}{result.Image.Trim('/')}";
    }

    public StaticItem? Get(string id)
    {
        id = id switch
        {
            "exalt" => "exalted",
            _ => id
        };

        return ByIds.GetValueOrDefault(id);
    }

    public StaticItem? Get(ItemHeader itemHeader)
    {
        var text = itemHeader.Name ?? itemHeader.Type ?? itemHeader.ApiType;
        if (text == null) return null;

        return ByTexts.GetValueOrDefault(text);
    }
}
