using System.Collections.Generic;
using System.ComponentModel;
using Cornebre.Maginot.Cards;
using Cornebre.Maginot.External;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Features;


public class KnowledgeManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{
	
	public KnowledgeManager(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		
		/*
		* There are times in which you need to hook code onto after a method.
		* Harmony allows us to achieve this on any method.
		* It is advised to name arguments for Harmony patches, to make them unambiguous to the reader.
		*/
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
			ModEntry.Instance.Logger.LogInformation("Do I enter this?");
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















// [HarmonyPatch(typeof(Ship))]
// public static class MaginotManagerArtilleryBank
// {
// 	[HarmonyPostfix]
// 	[HarmonyPatch(nameof(Ship.OnBeginTurn))]
// 	public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
// 	{
// 		int maginotArtillery = __instance.Get(ModEntry.Instance.MaginotManagerArtilleryBank.Status);
// 		if (maginotArtillery > 0)
// 		{

// 			c.QueueImmediate(new AAddCard
// 			{
// 				card = new MAginotCardArtilleryShell(),
// 				amount = maginotArtillery,
// 				destination = CardDestination.Hand
// 			});
// 		}
// 	}

// 	public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args) {
// 		if (args.Status == Onslaught) return [
// 			..args.Tooltips,
// 			new TTCard {
// 				card = new BurstShotCard()
// 			}
// 		];
// 		return args.Tooltips;
// 	}
// }