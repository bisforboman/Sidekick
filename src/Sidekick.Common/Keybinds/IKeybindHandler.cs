using Sidekick.Common.Initialization;

namespace Sidekick.Common.Keybinds;

public interface IKeybindHandler : IInitializableService
{
    /// <summary>
    /// Gets the keybinds that this handler handles.
    /// </summary>
    /// <returns>The list of keybinds.</returns>
    List<string?> Keybinds { get; }

    /// <summary>
    ///     When a keypress occurs, check if this keybind should be executed
    /// </summary>
    /// <param name="keybind">The keybind that was pressed</param>
    /// <returns>True if we need to execute this keybind</returns>
    bool IsValid(string keybind);

    /// <summary>
    ///     Executes when a valid keybind is detected
    /// </summary>
    /// <param name="keybind">The keybind that was pressed</param>
    /// <returns>A task</returns>
    Task Execute(string keybind);
}
