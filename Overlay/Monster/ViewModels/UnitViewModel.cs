using HunterPie.Core.Architecture;

namespace Arisen.Overlay.Monster.ViewModels;

internal class UnitViewModel(bool value) : Observable<bool>(value);