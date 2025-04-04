using System.Text.RegularExpressions;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Requirements;

public class RequirementsParser(IGameLanguageProvider gameLanguageProvider) : IRequirementsParser
{
    private Regex RequirementsPattern { get; } = gameLanguageProvider.Language.DescriptionRequirements.ToRegexStartOfLine();
    private Regex LevelPattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexStartOfLine();
    private static Regex IntPattern = new(@"\d+");

    public Common.Game.Items.Requirements? Parse(ParsingItem parsingItem)
    {
        foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
        {
            if (!block.TryParseRegex(RequirementsPattern, out _))
            {
                continue;
            }

            block.Parsed = true;
            return CreateRequirements(block);
        }

        return null;
    }

    private Common.Game.Items.Requirements CreateRequirements(ParsingBlock block) => new()
    {
        Level = FindValue(block, LevelPattern),
        Strength = null,
        Dexterity = null,
        Intelligence = null,
    };

    private static int? FindValue(ParsingBlock block, Regex pattern)
    {
        foreach (var line in block.Lines)
        {
            var match = pattern.Match(line.Text);
            if (match.Success)
            {
                var intValue = IntPattern.Match(match.Value);
                return int.Parse(intValue.Value);
            }
        }

        return null;
    }
}
