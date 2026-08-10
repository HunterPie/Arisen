using Arisen.Configuration;
using HunterPie.UI.Overlay.Enums;
using HunterPie.UI.Overlay.ViewModels;

namespace Arisen.Overlay.Monster.ViewModels;

internal class ArisenMonstersViewModel(
	IArisenConfiguration config
) : WidgetViewModel(config.MonsterWidget, "Dragons Dogma Monster Widget", WidgetType.ClickThrough)
{
	public ArisenMonsterViewModel? Target { get; set => SetValue(ref field, value); }
}