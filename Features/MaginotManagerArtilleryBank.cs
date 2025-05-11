using System.Collections.Generic;
using Cornebre.Maginot.Cards;
using Cornebre.Maginot.External;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Features;


public class MaginotManagerArtilleryBank : IKokoroApi.IV2.IStatusRenderingApi.IHook
{
	
	public MaginotManagerArtilleryBank(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin))
			//postfix: new HarmonyMethod(GetType(), nameof(AStatus_Begin_Postfix))
		);
	}
	
	public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
	{
		if (args.Status != ModEntry.Instance.MaginotManagerArtilleryBank.Status)
			return false;
		if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
			return false;

		if (args.Amount > 0) {
			args.Combat.Queue(new AAddCard {
				card = new MAginotCardArtilleryShell(),
				amount = args.Amount,
				destination = CardDestination.Hand,
				statusPulse = ModEntry.Instance.MaginotManagerArtilleryBank.Status
			});
		}
		return false;
	}
	
	public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args) {
		if (args.Status == ModEntry.Instance.MaginotManagerArtilleryBank.Status)
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