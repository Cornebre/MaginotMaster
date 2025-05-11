using Cornebre.Maginot.Cards;
using HarmonyLib;

namespace Cornebre.Maginot.Features;

[HarmonyPatch(typeof(Ship))]
public static class MaginotManagerArmored
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(Ship.OnBeginTurn))]
	public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
	{
		int maginotArmored = __instance.Get(ModEntry.Instance.MaginotManagerArmored.Status);
		if (maginotArmored > 0)
		{
			// c.QueueImmediate(new AAddCard
			// {
			// 	card = new MAginotCardArtilleryShell(),
			// 	amount = maginotArtillery,
			// 	destination = CardDestination.Hand
			// });
			if (__instance.Get(Status.timeStop) == 0) {
				__instance.Add(ModEntry.Instance.MaginotManagerArmored.Status, -1);
			}
		}
	}
}