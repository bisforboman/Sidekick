using System.Diagnostics;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Blazor.Initialization;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Wpf.Services;

public class WpfApplicationService
: IApplicationService, IDisposable
{
    private readonly IViewLocator viewLocator;
    private readonly IStringLocalizer<InitializationResources> resources;
    private readonly IBrowserProvider browserProvider;

    public WpfApplicationService(
        IViewLocator viewLocator,
        IStringLocalizer<InitializationResources> resources,
        IBrowserProvider browserProvider
)
    {
        this.viewLocator = viewLocator;
        this.resources = resources;
        this.browserProvider = browserProvider;
        Initialization = Initialize();
    }

    public bool SupportsKeybinds => true;

    private TaskbarIcon? Icon { get; set; }

    public int Priority => 9000;

    public Task Initialization { get; }

    private Task Initialize()
    {
        InitializeTray();
        return Task.CompletedTask;
    }

    private void InitializeTray()
    {
        Icon = new TaskbarIcon();
        Icon.Icon = new System.Drawing.Icon(System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot/favicon.ico"));
        Icon.ToolTipText = "Sidekick";
        Icon.ContextMenu = new ContextMenu();
        Icon.DoubleClickCommand = new SimpleCommand(() => viewLocator.Open("/settings"));

        if (Debugger.IsAttached)
        {
            AddTrayItem("Development", () => viewLocator.Open("/development"));
        }

        AddTrayItem("Sidekick - " + ((IApplicationService)this).GetVersion(), null, true);
        AddTrayItem(resources["Open_Website"],
                    () =>
                    {
                        browserProvider.OpenSidekickWebsite();
                        return Task.CompletedTask;
                    });
        AddTrayItem(resources["Settings"], () => viewLocator.Open("/settings"));
        AddTrayItem(resources["Exit"],
                    () =>
                    {
                        Shutdown();
                        return Task.CompletedTask;
                    });
    }

    private void AddTrayItem(string label, Func<Task>? onClick, bool disabled = false)
    {
        if (Icon?.ContextMenu == null) return;

        var menuItem = new MenuItem
        {
            Header = label,
            IsEnabled = !disabled,
        };

        if (onClick != null)
        {
            menuItem.Click += async (_, _) =>
            {
                await onClick();
            };
        }

        Icon.ContextMenu.Items.Add(menuItem);
    }

    public void Shutdown()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
        Environment.Exit(0);
    }

    public void Dispose()
    {
        if (Icon != null)
        {
            Icon.Dispose();
        }
    }
}
