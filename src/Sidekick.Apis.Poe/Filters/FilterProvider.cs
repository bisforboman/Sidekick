using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Filters.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Filters;

public class FilterProvider : IFilterProvider
{
    private readonly IPoeTradeClient poeTradeClient;
    private readonly IGameLanguageProvider gameLanguageProvider;
    private readonly ISettingsService settingsService;
    private readonly ICacheProvider cacheProvider;

    public FilterProvider(
        IPoeTradeClient poeTradeClient,
        IGameLanguageProvider gameLanguageProvider,
        ISettingsService settingsService,
        ICacheProvider cacheProvider)
    {
        this.poeTradeClient = poeTradeClient;
        this.gameLanguageProvider = gameLanguageProvider;
        this.settingsService = settingsService;
        this.cacheProvider = cacheProvider;

        Initialization = Initialize();
    }

    public List<ApiFilterOption> TypeCategoryOptions { get; private set; } = [];
    public List<ApiFilterOption> TradePriceOptions { get; private set; } = [];
    public List<ApiFilterOption> TradeIndexedOptions { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    public Task Initialization { get; }

    /// <inheritdoc/>
    private async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();
        var cacheKey = $"{game.GetValueAttribute()}_Filters";

        var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiFilter>(game, gameLanguageProvider.Language, "data/filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });

        TypeCategoryOptions = result.Result
            .First(x => x.Id == "type_filters").Filters
            .First(x => x.Id == "category").Option!.Options;

        TradePriceOptions = result.Result
            .First(x => x.Id == "trade_filters").Filters
            .First(x => x.Id == "price").Option!.Options;

        TradeIndexedOptions = result.Result
            .First(x => x.Id == "trade_filters").Filters
            .First(x => x.Id == "indexed").Option!.Options;
    }

    public string? GetPriceOption(string? price) =>
        TradePriceOptions.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) =>
        TradeIndexedOptions.SingleOrDefault(x => x.Id == id)?.Id;
}
