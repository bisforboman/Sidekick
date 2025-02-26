using Sidekick.Apis.Poe.Parser;

namespace Sidekick.Apis.Poe.Modifiers.Models;

internal record ModifierMatch(ParsingBlock Block, IReadOnlyCollection<ParsingLine> Lines, IReadOnlyCollection<ModifierPattern> Patterns);
