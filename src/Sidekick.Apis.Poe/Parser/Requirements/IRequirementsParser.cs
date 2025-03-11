namespace Sidekick.Apis.Poe.Parser.Requirements;

public interface IRequirementsParser
{
    Common.Game.Items.Requirements? Parse(ParsingItem parsingItem);
}
