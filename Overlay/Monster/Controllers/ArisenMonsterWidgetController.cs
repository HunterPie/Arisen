using Arisen.Configuration;
using Arisen.Configuration.Overlay;
using Arisen.Overlay.Monster.ViewModels;
using HunterPie.Core.Client.Configuration.Enums;
using HunterPie.Core.Game;
using HunterPie.Core.Game.Entity.Enemy;
using HunterPie.Core.Game.Enums;
using HunterPie.Core.Game.Events;
using HunterPie.Core.Game.Services.Monster;
using HunterPie.Core.Game.Services.Monster.Events;
using HunterPie.DI;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Threading;

namespace Arisen.Overlay.Monster.Controllers;

internal class ArisenMonsterWidgetController(
	IContext context,
	ITargetDetectionService targetDetectionService,
	ArisenMonstersViewModel viewModel,
	IArisenConfiguration configuration
) : IDisposable
{
	private readonly ArisenMonsterWidgetConfiguration config = configuration.MonsterWidget;
	private readonly ConcurrentDictionary<IMonster, ArisenMonsterController> _monsters = new();

	public ArisenMonstersViewModel ViewModel => viewModel;

	public void Initialize()
	{
		config.TargetMode.PropertyChanged += OnTargetModeChanged;
		context.Game.OnMonsterSpawn += OnMonsterSpawn;
		context.Game.OnMonsterDespawn += OnMonsterDespawn;
		targetDetectionService.OnTargetChanged += OnInferredTargetChanged;

		Load();
	}

	public void Dispose()
	{
		config.TargetMode.PropertyChanged -= OnTargetModeChanged;
		context.Game.OnMonsterSpawn -= OnMonsterSpawn;
		context.Game.OnMonsterDespawn -= OnMonsterDespawn;
		targetDetectionService.OnTargetChanged -= OnInferredTargetChanged;

		foreach (IMonster monster in _monsters.Keys.ToImmutableList())
			DestroyMonster(monster);
	}

	private void Load()
	{
		foreach (IMonster monster in context.Game.Monsters)
			CreateMonster(monster);
	}

	private void OnTargetChanged(object? sender, MonsterTargetEventArgs e) => UpdateTarget();

	private void OnInferredTargetChanged(object? sender, InferTargetChangedEventArgs e) => UpdateTarget();

	private void OnMonsterSpawn(object? sender, IMonster e) => CreateMonster(e);

	private void OnMonsterDespawn(object? sender, IMonster e) => DestroyMonster(e);

	private void OnTargetModeChanged(object? _, PropertyChangedEventArgs __) => UpdateTarget();

	private void CreateMonster(IMonster monster)
	{
		monster.OnTargetChange += OnTargetChanged;

		var controller = new ArisenMonsterController(
			dispatcher: DependencyContainer.Get<Dispatcher>(),
			context: monster,
			viewModel: new ArisenMonsterViewModel()
		);

		if (!_monsters.TryAdd(monster, controller))
			return;

		controller.Initialize();

		viewModel.Target = controller.ViewModel;
	}

	private void DestroyMonster(IMonster monster)
	{
		if (!_monsters.TryRemove(monster, out ArisenMonsterController? controller))
			return;

		ArisenMonsterViewModel vm = controller.ViewModel;

		if (viewModel.Target == vm)
			viewModel.Target = null;

		controller.Dispose();
	}

	private void UpdateTarget()
	{
		IMonster? target = config.TargetMode.Value switch
		{
			TargetModeType.LockOn => context.Game.Monsters.FirstOrDefault(
				static (it) => it.Target == Target.Self
			),
			TargetModeType.MapPin or TargetModeType.AutoQuest => context.Game.Monsters.FirstOrDefault(
				static (it) => it.ManualTarget == Target.Self
			),
			TargetModeType.Infer => targetDetectionService.Target,
			_ => null
		};

		if (target is not { })
			return;

		if (!_monsters.TryGetValue(target, out ArisenMonsterController? controller))
			return;

		viewModel.Target = controller.ViewModel;
	}
}