using HarmonyLib;

namespace Cornebre.Maginot.Features;

[HarmonyPatch(typeof(Ship))]
public static class MaginotManagerAutoTempShield
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(Ship.OnBeginTurn))]
	public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
	{
		int maginotAutoShield = __instance.Get(ModEntry.Instance.MaginotManagerAutoTempShield.Status);
		if (maginotAutoShield > 0)
		{

			c.QueueImmediate(new AStatus {
				status = Status.tempShield,
				targetPlayer = __instance.isPlayerShip,
				statusAmount = maginotAutoShield,
				statusPulse = ModEntry.Instance.MaginotManagerAutoTempShield.Status
			});
		}
	}
}