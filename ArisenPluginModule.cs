using Arisen.Configuration.Plugin;
using Arisen.Overlay.Monster.Controllers;
using Arisen.Overlay.Monster.ViewModels;
using HunterPie.Core.Plugins.Configuration;
using HunterPie.Core.Plugins.DI;
using HunterPie.DI;

namespace Arisen;

public class ArisenPluginModule : IPluginModule
{
    public PluginConfiguration Configuration { get; } = new ArisenConfigurationV1();

    public void Register(IScopedDependencyRegistry registry)
    {
        registry
            .WithFactory<ArisenPlugin>()
            .WithFactory<ArisenMonsterViewModel>()
            .WithFactory<ArisenMonstersViewModel>()
            .WithSingle<ArisenMonsterWidgetController>();
    }
}
