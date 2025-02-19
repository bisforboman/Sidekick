using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class LastTwelveHoursProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    public override List<Category> ValidCategories { get; } = [.. Enum.GetValues<Category>().Except([Category.Unknown])];

    public override void Initialize()
    {
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue) => new(this)
    {
        Text = gameLanguageProvider.Language.DescriptionLastTwelveHours,
        Checked = false,
    };

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        searchFilters.GetOrCreateTradeFilters().Filters.Indexed = new TimeFilterValue(TimeOptions.UpTo12HoursAgo);
    }
}
