using System.Text.RegularExpressions;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Sockets;

public class SocketParser(IGameLanguageProvider gameLanguageProvider) : ISocketParser
{
    private Regex Pattern { get; } = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWAS]+)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)\\ ?([-RGBWAS]*)");

    public List<Socket> Parse(ParsingItem parsingItem) => [.. Parse2(parsingItem)];

    private IEnumerable<Socket> Parse2(ParsingItem parsingItem)
    {
        if (!parsingItem.TryParseRegex(Pattern, out var match))
        {
            yield break;
        }

        var groups = match.Groups.Values
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .Skip(1)
            .Select((x, index) => new
            {
                Value = x.Value.Replace("-", "").Trim(),
                Index = index,
            })
            .ToList();

        foreach (var group in groups)
        {
            foreach (var ch in group.Value)
            {
                yield return MapSocket(group.Index, ch);
            }
        }
    }

    private static Socket MapSocket(int groupIndex, char ch) => ch switch
    {
        'B' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.Blue
        },
        'G' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.Green
        },
        'R' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.Red
        },
        'W' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.White
        },
        'A' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.Abyss
        },
        'S' => new Socket()
        {
            Group = groupIndex,
            Colour = SocketColour.Soulcore
        },
        _ => throw new UnparsableException(),
    };
}
