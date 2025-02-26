using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Modifiers;

public class ModifierParser
(
    IModifierProvider modifierProvider,
    IFuzzyService fuzzyService
) : IModifierParser
{
    private static readonly Regex CleanOriginalTextPattern = new(" \\((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible)\\)$");

    /// <inheritdoc/>
    public List<ModifierLine> Parse(ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category is Category.DivinationCard or Category.Gem)
        {
            return [];
        }

        return MatchModifiers(parsingItem)
            .Select(CreateModifierLine)
            // Trim modifier lines
            .Where(x => x.Modifiers.Count > 0)
            // Order the mods by the order they appear on the item.
            .OrderBy(x => parsingItem.Text.IndexOf(x.Text, StringComparison.InvariantCulture))
            .ToList();
    }

    private ModifierLine CreateModifierLine(ModifierMatch match)
    {
        var text = CreateString(match);
        var modifierLine = new ModifierLine(CleanOriginalTextPattern.Replace(text, string.Empty));

        var fuzzyLine = fuzzyService.CleanFuzzyText(text);
        var patterns = match.Patterns.OrderByDescending(x => Fuzz.Ratio(fuzzyLine, x.FuzzyText)).ToList();

        foreach (var pattern in patterns)
        {
            if (!modifierLine.Modifiers.Any(x => x.Id == pattern.Id))
            {
                modifierLine.Modifiers.Add(new()
                {
                    Text = pattern.Text,
                    Id = pattern.Id,
                    Category = pattern.Category,
                });
            }
        }

        if (modifierLine.Modifiers.All(x => x.Category is ModifierCategory.Pseudo))
        {
            modifierLine.Text = modifierLine.Modifiers.FirstOrDefault()?.Text ?? modifierLine.Text;
        }

        if (patterns is [var firstPattern, ..])
        {
            ParseModifierValue(modifierLine, firstPattern);
        }

        return modifierLine;
    }

    private static string CreateString(ModifierMatch match)
    {
        var text = new StringBuilder();
        foreach (var line in match.Lines)
        {
            if (text.Length is not 0)
            {
                text.Append('\n');
            }

            text.Append(line.Text);
            line.Parsed = true;
        }

        return text.ToString();
    }

    private IEnumerable<ModifierMatch> MatchModifiers(ParsingItem parsingItem)
    {
        foreach (var block in parsingItem.Blocks.Where(x => !x.AnyParsed))
        {
            for (var lineIndex = 0; lineIndex < block.Lines.Count; lineIndex++)
            {
                var line = block.Lines[lineIndex];
                if (line.Parsed)
                {
                    continue;
                }

                var patterns = GetPatterns(line, block, lineIndex, parsingItem);

                if (patterns.Count is 0)
                {
                    continue;
                }

                var maxLineCount = patterns.Max(x => x.LineCount);
                lineIndex += maxLineCount - 1; // Increment the line index by one less of the pattern count. The default lineIndex++ will take care of the remaining increment.
                yield return new ModifierMatch(block, [.. block.Lines.Skip(lineIndex).Take(maxLineCount)], patterns);
            }
        }
    }

    private IReadOnlyCollection<ModifierPattern> GetPatterns(ParsingLine line, ParsingBlock block, int lineIndex, ParsingItem parsingItem)
    {
        var allAvailablePatterns = GetAllAvailablePatterns(parsingItem);
        IReadOnlyCollection<ModifierPattern> patterns = [ .. MatchModifierPatterns(line, block, lineIndex, allAvailablePatterns)];

        if (patterns.Count is not 0)
        {
            return patterns;
        }

        // If we reach this point we have not found the modifier through traditional Regex means.
        // Text from the game sometimes differ from the text from the API. We do a fuzzy search here to find the most common text.
        return [ .. MatchModifierFuzzily(line, allAvailablePatterns)];
    }

    private static IEnumerable<ModifierPattern> MatchModifierPatterns(ParsingLine line, ParsingBlock block, int lineIndex, IReadOnlyCollection<ModifierPattern> allAvailablePatterns)
    {
        var remainingLines = block.Lines.Skip(lineIndex);
        foreach (var pattern in allAvailablePatterns)
        {
            var isMultilineModifierMatch = pattern.LineCount > 1 && pattern.Pattern.IsMatch(string.Join('\n', remainingLines.Take(pattern.LineCount)));
            var isSingleModifierMatch = pattern.Pattern.IsMatch(line.Text);

            if (isMultilineModifierMatch || isSingleModifierMatch)
            {
                yield return pattern;
            }
        }
    }

    private IEnumerable<ModifierPattern> MatchModifierFuzzily(ParsingLine line, IReadOnlyCollection<ModifierPattern> allAvailablePatterns)
    {
        foreach (var (ratio, pattern) in GetReliablePatterns(line, allAvailablePatterns).OrderByDescending(x => x.Ratio))
        {
            yield return pattern;
        }
    }

    private ConcurrentBag<(int Ratio, ModifierPattern Pattern)> GetReliablePatterns(ParsingLine line, IReadOnlyCollection<ModifierPattern> allAvailablePatterns)
    {
        var results = new ConcurrentBag<(int Ratio, ModifierPattern Pattern)>();
        var fuzzyLine = fuzzyService.CleanFuzzyText(line.Text);
        Parallel.ForEach(
            allAvailablePatterns,
            (x) =>
            {
                var ratio = Fuzz.Ratio(fuzzyLine, x.FuzzyText, FuzzySharp.PreProcess.PreprocessMode.None);

                if (ratio > 75)
                {
                    results.Add((ratio, x));
                }
            });

        return results;
    }

    private IReadOnlyCollection<ModifierPattern> GetAllAvailablePatterns(ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category is Category.Sanctum)
        {
            return [ .. modifierProvider.Patterns[ModifierCategory.Sanctum]];
        }

        if (parsingItem.Header?.Category is Category.Map && parsingItem.Header.ItemCategory is "map.tablet")
        {
            return [ .. modifierProvider.Patterns[ModifierCategory.Implicit],
                     .. modifierProvider.Patterns[ModifierCategory.Explicit]];
        }

        return [ .. modifierProvider.Patterns.SelectMany(x => x.Value)];
    }

    private static void ParseModifierValue(ModifierLine modifierLine, ModifierPattern pattern)
    {
        switch (pattern)
        {
            case { IsOption: true }:
                modifierLine.OptionValue = pattern.Value;
                return;

            case { Value: int value }:
                modifierLine.Values.Add(value);
                return;

            default:
                modifierLine.Values.AddRange(CreateDoubles(modifierLine.Text));
                return;
        }
    }

    private static IEnumerable<double> CreateDoubles(string text)
    {
        var matches = new Regex("([-+0-9,.]+)").Matches(text);
        foreach (Match match in matches)
        {
            if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
            {
                yield return parsedValue;
            }
        }
    }
}
