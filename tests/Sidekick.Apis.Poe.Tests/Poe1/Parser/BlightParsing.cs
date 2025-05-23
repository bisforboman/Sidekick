using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class BlightParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBlightedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Atoll Map
--------
Map Tier: 14
Atlas Region: Tirn's End
--------
Item Level: 81
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 20% value (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("map", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal("Blighted Atoll Map", actual.Header.Type);
        Assert.Equal("Blighted Atoll Map", actual.Header.ApiText);
        Assert.Equal("Atoll Map", actual.Header.ApiType);
        Assert.Equal("blighted", actual.Header.ApiDiscriminator);
        Assert.Equal(14, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }

    [Fact]
    public void ParseSuperiorBlightedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Superior Blighted Shore Map
--------
Map Tier: 6
Item Quantity: +5% (augmented)
Quality: +5% (augmented)
--------
Item Level: 74
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("map", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal("Blighted Shore Map", actual.Header.ApiText);
        Assert.Equal("Shore Map", actual.Header.ApiType);
        Assert.Equal("blighted", actual.Header.ApiDiscriminator);
        Assert.Null(actual.Header.Name);
        Assert.Equal(6, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }

    [Fact]
    public void ClearOil()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Clear Oil
--------
Stack Size: 5/10
--------
Can be combined with other Oils at Cassia to Enchant Rings or Amulets, or to modify Blighted Maps.
Shift click to unstack.
--------
Note: ~price 1 blessed
");

        Assert.Equal("currency", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Currency, actual.Header.Category);
        Assert.Equal("Clear Oil", actual.Header.ApiType);
    }

    [Fact]
    public void BlightedSpiderForestMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Nightmare Spires
Blighted Spider Forest Map
--------
Map Tier: 2
Atlas Region: Lex Proxima
Item Quantity: +55% (augmented)
Item Rarity: +32% (augmented)
Monster Pack Size: +21% (augmented)
--------
Item Level: 69
--------
Area is infested with Fungal Growths
Map's Item Quantity Modifiers also affect Blight Chest count at 20% value (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Monsters deal 54% extra Physical Damage as Lightning
Unique Boss has 25% increased Life
Unique Boss has 45% increased Area of Effect
Slaying Enemies close together has a 13% chance to attract monsters from Beyond
Players have 20% less Recovery Rate of Life and Energy Shield
--------
Travel to this Map by using it in a personal Map Device.Maps can only be used once.
");

        Assert.Equal("map", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("Blighted Spider Forest Map", actual.Header.ApiText);
        Assert.Equal("Spider Forest Map", actual.Header.ApiType);
        Assert.Equal("blighted", actual.Header.ApiDiscriminator);
        Assert.Equal("Nightmare Spires", actual.Header.Name);
        Assert.True(actual.Properties.Blighted);
    }
}
