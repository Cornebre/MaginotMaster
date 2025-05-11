using HarmonyLib;

namespace Cornebre.Maginot.Features;

[HarmonyPatch(typeof(Ship))]
public static class MaginotManagerAutoShield
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(Ship.OnBeginTurn))]
	public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
	{
		int maginotAutoShield = __instance.Get(ModEntry.Instance.MaginotManagerAutoShield.Status);
		if (maginotAutoShield > 0)
		{

			c.QueueImmediate(new AStatus {
				status = Status.shield,
				targetPlayer = __instance.isPlayerShip,
				statusAmount = maginotAutoShield,
				statusPulse = ModEntry.Instance.MaginotManagerAutoShield.Status
			});
		}
	}
}