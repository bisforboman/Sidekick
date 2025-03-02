using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeNinja;

public interface IPoeNinjaClient : IInitializableService
{
    Task<NinjaPrice?> GetPriceInfo(
        string? englishName,
        string? englishType,
        Category category,
        int? gemLevel = null,
        int? mapTier = null,
        bool? isRelic = false,
        int? numberOfLinks = null);

    Task<NinjaPrice?> GetClusterPrice(
        string englishGrantText,
        int passiveCount,
        int itemLevel);

    Task<Uri> GetDetailsUri(NinjaPrice price);
}
