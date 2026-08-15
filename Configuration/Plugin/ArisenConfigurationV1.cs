using Arisen.Configuration.Overlay;
using HunterPie.Core.Plugins.Configuration;

namespace Arisen.Configuration.Plugin;

internal class ArisenConfigurationV1 : PluginConfiguration, IArisenConfiguration
{
    public override int Version => 1;

    public ArisenMonsterWidgetConfiguration MonsterWidget { get; } = new();
}
