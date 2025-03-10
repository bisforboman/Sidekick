namespace Sidekick.Apis.Poe.Parser.Requirements;

public interface IRequirementsParser
{
    Requirements? Parse(ParsingItem parsingItem);
}
