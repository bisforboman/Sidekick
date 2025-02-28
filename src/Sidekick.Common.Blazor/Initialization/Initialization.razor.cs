using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Cache;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Blazor.Initialization;

public partial class Initialization : SidekickView
{
    [Inject]
    private IStringLocalizer<InitializationResources> Resources { get; set; } = null!;

    [Inject]
    private ILogger<Initialization> Logger { get; set; } = null!;

    [Inject]
    private IApplicationService ApplicationService { get; set; } = null!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    private ISettingsService SettingsService { get; set; } = null!;

    [Inject]
    private ICacheProvider CacheProvider { get; set; } = null!;

    private int Count { get; set; }

    private int Completed { get; set; }

    private string? Step { get; set; }

    private int Percentage { get; set; }

    private string? WelcomeMessage { get; set; }

    public Task? InitializationTask { get; set; }

    public override SidekickViewType ViewType => SidekickViewType.Modal;

    private int TimeLeftToCloseView { get; set; }

    protected override async Task OnInitializedAsync()
    {
        InitializationTask = Handle();
        var keyOpenPriceCheck = await SettingsService.GetString(SettingKeys.KeyOpenPriceCheck);
        var keyClose = await SettingsService.GetString(SettingKeys.KeyClose);
        WelcomeMessage = string.Format(Resources["Notification"], keyOpenPriceCheck.ToKeybindString(), keyClose.ToKeybindString());
        await base.OnInitializedAsync();
        await InitializationTask;
    }

    public async Task Handle()
    {
        try
        {
            Completed = 0;
            Count = SidekickConfiguration.InitializableServices.Count;
            var version = ApplicationService.GetVersion();
            var previousVersion = await SettingsService.GetString(SettingKeys.Version);
            if (version != previousVersion)
            {
                await CacheProvider.Clear();
                await SettingsService.Set(SettingKeys.Version, version);
            }

            // Report initial progress
            await ReportProgress();

            var services = SidekickConfiguration.InitializableServices.Select(serviceType =>
                {
                    var service = ServiceProvider.GetRequiredService(serviceType);
                    return service as IInitializableService;
                })
                .Where(x => x != null)
                .Select(x => x!)
                .OrderBy(x => x.Priority);

            foreach (var service in services)
            {
                Logger.LogInformation($"[Initialization] Initializing {service.GetType().FullName}");
                await service.Initialize();
                Completed += 1;
                await ReportProgress();
            }

            // If we have a successful initialization, we delay for half a second to show the
            // "Ready" label on the UI before closing the view
            Completed = Count;

            await StartCountdownToClose();
            await ReportProgress();
        }
        catch (SidekickException e)
        {
            await SettingsService.Set(SettingKeys.LanguageParser, null);
            e.Actions = ExceptionActions.ExitApplication;
            throw;
        }
    }

    private async Task StartCountdownToClose()
    {
        TimeLeftToCloseView = Debugger.IsAttached ? 1 : 4;

        while (TimeLeftToCloseView > 0)
        {
            StateHasChanged();
            await Task.Delay(1000);
            TimeLeftToCloseView--;
        }

        if (Debugger.IsAttached)
        {
            NavigationManager.NavigateTo("/development");
        }
        else
        {
            await CurrentView.Close();
        }
    }

    private Task ReportProgress()
    {
        return InvokeAsync(() =>
        {
            Percentage = Count == 0 ? 0 : Completed * 100 / Count;
            if (Percentage >= 100)
            {
                Step = Resources["Ready"];
                Percentage = 100;
            }
            else
            {
                Step = Resources["Title", Completed, Count];
            }

            StateHasChanged();
            return Task.Delay(100);
        });
    }

    public void Exit()
    {
        ApplicationService.Shutdown();
    }
}
