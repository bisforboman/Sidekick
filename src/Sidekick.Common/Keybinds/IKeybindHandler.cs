using Sidekick.Common.Initialization;

namespace Sidekick.Common.Keybinds;

public interface IKeybindHandler : IInitializableService
{
    bool UsesKeybind(string keybind);
    Task Execute(string keybind);
}
