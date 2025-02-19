using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public class TimeFilterValue
{
    public string Option { get; set; } = null!;

    public TimeFilterValue()
    {
    }

    public TimeFilterValue(TimeOptions timeOptions)
    {
        Option = timeOptions switch
        {
            TimeOptions.UpToAnHourAgo => "1hour",
            TimeOptions.UpTo3HoursAgo => "3hours",
            TimeOptions.UpTo12HoursAgo => "12hours",
            TimeOptions.UpToADayAgo => "1day",
            TimeOptions.UpTo3DaysAgo => "3days",
            TimeOptions.UpToAWeekAgo => "1week",
            TimeOptions.UpTo2WeeksAgo => "2weeks",
            TimeOptions.UpToAMonthAgo => "1month",
            TimeOptions.UpTo2MonthsAgo => "2months",
            _ => throw new InvalidDataException($"'{timeOptions:G}' is not mapped"),
        };
    }
}

public class StatFilterValue
{
    internal StatFilterValue()
    {
    }

    public StatFilterValue(string? option)
    {
        Option = option;
    }

    public StatFilterValue(IntPropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(DoublePropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
        Min = filter.Min;
        Max = filter.Max;
    }

    public StatFilterValue(ModifierFilter filter)
    {
        Option = filter.Line.OptionValue;
        Min = filter.Min;
        Max = filter.Max;
    }

    public object? Option { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double? Weight { get; set; }
}
