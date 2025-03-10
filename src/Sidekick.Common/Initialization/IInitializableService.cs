namespace Sidekick.Common.Initialization;

/// <summary>
///     Interface for a service that needs to be initialized during startup.
/// </summary>
public interface IInitializableService
{
    /// <summary>
    ///     Initializes the service during startup.
    /// </summary>
    /// <returns>A task.</returns>
    Task Initialization { get; }
}
