using System.Collections.Generic;
using Cornebre.Maginot.Cards;
using Cornebre.Maginot.External;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Features;

public class MaginotManagerAutoShield : IKokoroApi.IV2.IStatusRenderingApi.IHook, IKokoroApi.IV2.IStatusLogicApi.IHook
{
	
	public MaginotManagerAutoShield(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this);
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin))
		);
	}
	
	public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
	{
		if (args.Status != ModEntry.Instance.MaginotManagerAutoShield.Status)
			return false;
		if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
			return false;

		if (args.Amount > 0) {
			args.Combat.Queue(new AStatus {
				status = Status.shield,
				targetPlayer = args.Ship.isPlayerShip,
				statusAmount = args.Amount,
				statusPulse = ModEntry.Instance.MaginotManagerAutoShield.Status
			});
		}
		return false;
	}
	
	public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args) {
		if (args.Status == ModEntry.Instance.MaginotManagerAutoShield.Status)
		{
			return [
				..args.Tooltips,
				new TTCard {
					card = new MAginotCardArtilleryShell()
				}
		];}
		return args.Tooltips;
	}

}