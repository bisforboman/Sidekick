using Sidekick.Common.Platform;

namespace Sidekick.Web.Services;

public class WebApplicationService : IApplicationService
{
    public bool SupportsKeybinds => false;

    public Task Initialization { get; } = Task.CompletedTask;

    public void Shutdown()
    {
        Environment.Exit(0);
    }
}
