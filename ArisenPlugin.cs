using Arisen.Configuration;
using Arisen.Overlay.Monster.Controllers;
using Arisen.Overlay.Monster.ViewModels;
using Arisen.Overlay.Monster.Views;
using HunterPie.Core.Plugins.Entity;
using HunterPie.UI.Overlay.Service;
using HunterPie.UI.Overlay.Views;

namespace Arisen;

internal class ArisenPlugin(
    IOverlay overlay,
    IWidgetProvider provider,
    IArisenConfiguration configuration,
    ArisenMonsterWidgetController controller
) : IPlugin
{
    private WidgetView? _view;

    public Task InitializeAsync()
    {
        if (!configuration.MonsterWidget.Initialize)
            return Task.CompletedTask;

        provider.Bind<ArisenMonstersViewModel, ArisenMonstersView>();

        controller.Initialize();

        _view = overlay.Register(
            viewModel: controller.ViewModel
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_view is { })
            overlay.Unregister(_view);

        controller.Dispose();
    }
}
