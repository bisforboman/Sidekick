using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Pseudo;
using Sidekick.Apis.Poe.Parser.Requirements;
using Sidekick.Apis.Poe.Parser.Sockets;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IModifierParser modifierParser,
    IPseudoParser pseudoParser,
    IRequirementsParser requirementsParser,
    ClusterJewelParser clusterJewelParser,
    IApiInvariantItemProvider apiInvariantItemProvider,
    ISocketParser socketParser,
    IPropertyParser propertyParser,
    IHeaderParser headerParser
) : IItemParser
{
    public Item ParseItem(string itemText)
    {
        if (string.IsNullOrEmpty(itemText))
        {
            throw new UnparsableException();
        }

        try
        {
            var parsingItem = new ParsingItem(itemText);
            var itemHeader = headerParser.Parse(parsingItem);
            if (string.IsNullOrEmpty(itemHeader.ApiName) && string.IsNullOrEmpty(itemHeader.ApiType))
            {
                throw new UnparsableException();
            }

            var invariant = GetInvariant(itemHeader);

            // Order of parsing is important
            requirementsParser.Parse(parsingItem);
            var sockets = socketParser.Parse(parsingItem);
            var properties = propertyParser.Parse(parsingItem, itemHeader);
            var modifierLines = modifierParser.Parse(parsingItem, itemHeader);
            propertyParser.ParseAfterModifiers(parsingItem, properties, modifierLines, itemHeader);
            var pseudoModifiers = pseudoParser.Parse(modifierLines);
            var clusterInformation = clusterJewelParser.Parse(itemHeader, modifierLines);

            return new Item
            {
                Invariant = invariant,
                Header = itemHeader,
                Properties = properties,
                Sockets = sockets,
                ModifierLines = modifierLines,
                PseudoModifiers = pseudoModifiers,
                Text = parsingItem.Text,
                AdditionalInformation = clusterInformation,
            };
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw;
        }
    }

    private ItemHeader? GetInvariant(ItemHeader itemHeader)
    {
        if (itemHeader.ApiItemId != null && apiInvariantItemProvider.IdDictionary.TryGetValue(itemHeader.ApiItemId, out var invariantMetadata))
        {
            return invariantMetadata.ToHeader();
        }

        return null;
    }
}
