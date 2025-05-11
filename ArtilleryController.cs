using System;
using HarmonyLib;

namespace Cornebre.Maginot;

[HarmonyPatch(typeof(AAttack))]
public class ArtilleryShotController
{
	public static readonly string key = "AttackArtilleryKey";
	
	[HarmonyPrefix]
	[HarmonyPatch(nameof(AAttack.Begin))]
	public static void HarmonyPrefix_ArtilleryShotPatch(AAttack __instance, out StuffBase? __state, G g, State s, Combat c)
	{
		__state = null;

		if (ModEntry.Instance.Helper.ModData.GetModDataOrDefault(__instance, key, false))
		{
			Ship ship = !__instance.targetPlayer ? s.ship : c.otherShip;
			if (ship == null || ship.hull <= 0 || (__instance.fromDroneX.HasValue && !c.stuff.ContainsKey(__instance.fromDroneX.Value)))
			{
				return;
			}
			int? num = __instance.GetFromX(s,c);

			if (num.HasValue)
			{
				num += ship.x;
				if (c.stuff.TryGetValue(num.Value, out StuffBase? value))
				{
					__state = Mutil.DeepCopy(value);
					c.stuff.Remove(num.Value);
				}
			}
		}
	}

	[HarmonyFinalizer]
	[HarmonyPatch(nameof(AAttack.Begin))]
	public static void HarmonyFinalizer_ArtilleryShotPatch(AAttack __instance, in StuffBase? __state, G g, State s, Combat c)
	{
		if (ModEntry.Instance.Helper.ModData.GetModDataOrDefault(__instance, key, false))
		{
			Ship ship = !__instance.targetPlayer ? s.ship : c.otherShip;
			int? num = __instance.GetFromX(s,c);
			if (num.HasValue && __state != null)
			{
				num += ship.x;
				c.stuff[num.Value] = __state;
			}
		}
	}
}