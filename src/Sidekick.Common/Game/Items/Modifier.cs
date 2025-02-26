namespace Sidekick.Common.Game.Items;

public class Modifier
{
    public string? Id { get; init; }
    public string? Tier { get; init; }
    public string? TierName { get; set; }
    public ModifierCategory Category { get; init; }
    public required string Text { get; init; }

    public override string ToString() => $"[{Id}] {Text}";
}
