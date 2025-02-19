using System.Reflection;

namespace Sidekick.Common;

/// <summary>
///     Configuration class for the application.
/// </summary>
public class SidekickConfiguration
{
    /// <summary>
    ///     Gets or sets a list of modules.
    /// </summary>
    public List<Assembly> Modules { get; } = [];
}
