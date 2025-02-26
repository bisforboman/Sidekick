using Sidekick.Common.Settings;

namespace Sidekick.Common.Keybinds;

public abstract class KeybindHandler : IKeybindHandler
{
    protected KeybindHandler(ISettingsService settingsService)
    {
        settingsService.OnSettingsChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged()
    {
        _ = Task.Run(
            async () =>
            {
                Keybinds = await GetKeybinds();
            });
    }

    /// <inheritdoc />
    public List<string?> Keybinds { get; private set; } = [];

    public int Priority => 0;

    public async Task Initialize()
    {
        Keybinds = await GetKeybinds();
    }

    /// <summary>
    /// Gets the keybinds that this handler handles.
    /// </summary>
    /// <returns>The list of keybinds.</returns>
    protected abstract Task<List<string?>> GetKeybinds();

    /// <inheritdoc />
    public abstract bool IsValid(string keybind);

    /// <inheritdoc />
    public abstract Task Execute(string keybind);

}
