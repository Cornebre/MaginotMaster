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
			original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
			postfix: new HarmonyMethod(GetType(), nameof(AStatus_Begin_Postfix))
		);
	}
	
	public static void AStatus_Begin_Postfix(AStatus __instance, State s, Combat c)
	{
		if (__instance.status != ModEntry.Instance.MaginotManagerArtilleryBank.Status) return;
		
		var ship = __instance.targetPlayer ? s.ship : c.otherShip;
		int maginotArtillery = ship.Get(ModEntry.Instance.MaginotManagerArtilleryBank.Status);
		
		c.QueueImmediate([
			new AAddCard
			{
				card = new MAginotCardArtilleryShell(),
				amount = maginotArtillery,
				destination = CardDestination.Hand
			}
		]);
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