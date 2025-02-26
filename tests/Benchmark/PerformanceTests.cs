using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Requirements;
using Sidekick.Apis.Poe.Tests;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Database;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Benchmark;

public class PerformanceTests
{
    private IModifierParser PreviousModifierParser { get; }
    private IModifierParser NewModifierParser { get; }
    private ParsingItem ParsingItem { get; }

    public PerformanceTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLocalization()
            .AddSidekickCommon()
            .AddSidekickCommonDatabase(SidekickPaths.DatabasePath)
            .AddSidekickPoeApi()
            .AddSidekickPoeNinjaApi()
            .AddSidekickPoeWikiApi()
            .BuildServiceProvider();

        var requirementsParser = serviceProvider.GetRequiredService<IRequirementsParser>();
        var headerParser = serviceProvider.GetRequiredService<IHeaderParser>();
        // var socketParser = serviceProvider.GetRequiredService<ISocketParser>();
        // var propertyParser = serviceProvider.GetRequiredService<IPropertyParser>();

        var text = ResourceHelper.ReadFileContent("item.txt");

        ParsingItem = new ParsingItem(text);
        ParsingItem.Header = headerParser.Parse(ParsingItem);

        requirementsParser.Parse(ParsingItem);
        // var sockets = socketParser.Parse(parsingItem);
        // var properties = propertyParser.Parse(parsingItem);
        // var modifierLines = modifierParser.Parse(parsingItem);

        PreviousModifierParser = serviceProvider.GetRequiredKeyedService<IModifierParser>("Previous");
        NewModifierParser = serviceProvider.GetRequiredKeyedService<IModifierParser>("New");
    }

    [Benchmark]
    public List<ModifierLine> ModifierParser2() => PreviousModifierParser.Parse(ParsingItem);

    [Benchmark]
    public List<ModifierLine> ModifierParser() => NewModifierParser.Parse(ParsingItem);
}

public class Program
{
    public static void Main(string[] _) => BenchmarkRunner.Run<PerformanceTests>();
}
