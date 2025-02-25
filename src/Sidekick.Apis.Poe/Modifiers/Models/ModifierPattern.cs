using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Modifiers.Models;

public class ModifierPattern
{
    public required string Id { get; init; }

    public ModifierCategory Category { get; init; }

    public required bool IsOption { get; init; }

    public required string Text { get; init; }

    public required string FuzzyText { get; init; }

    public string? OptionText { get; init; }

    public int LineCount => OptionText != null ? (OptionText?.Split('\n').Length ?? 1) : (Text?.Split('\n').Length ?? 1);

    public required Regex Pattern { get; init; }

    public int? Value { get; init; }
}
