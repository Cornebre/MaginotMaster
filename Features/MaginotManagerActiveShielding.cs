using System.Collections.Generic;
using Cornebre.Maginot.External;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Features;

public class MaginotManagerActiveShielding : IKokoroApi.IV2.IStatusRenderingApi.IHook, IKokoroApi.IV2.IStatusLogicApi.IHook
{
	public static readonly string key = "BypassActiveShieldingKey";

	public MaginotManagerActiveShielding(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);
		ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this);
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin))
		);
	}
	
	public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
	{
		if (args.Status != ModEntry.Instance.MaginotManagerActiveShielding.Status)
			return false;
		if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
			return false;

		if (args.Amount > 0) {
			args.Amount -= 1;
		}
		return false;
	}

	[HarmonyPatch(typeof(AAttack))]
	public class MaginotActiveShieldingController
	{

		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		[HarmonyPatch(nameof(AAttack.ApplyAutododge))]
		public static void HarmonyPostfix_ActiveShieldingPatch(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
		{
			if (!__result && target.Get(ModEntry.Instance.MaginotManagerActiveShielding.Status) > 0 && !ModEntry.Instance.Helper.ModData.GetModDataOrDefault(__instance, key, false) && ray.hitShip)
			{
				c.QueueImmediate(new List<CardAction>
				{
					new AStatus {
						status = Status.shield,
						targetPlayer = target.isPlayerShip,
						statusAmount = 1,
						statusPulse = ModEntry.Instance.MaginotManagerActiveShielding.Status
					},
					__instance.BypassActiveShielding(true)
				});
				__result = true;
			}
		}
	}
}
