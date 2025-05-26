using System.Collections.Generic;
using Nickel;

namespace Cornebre.Maginot.Actions;

public class MaginotActionMaxShieldHint : AVariableHint
{
	public static Spr Spr;
	public override Icon? GetIcon(State s)
	{
		return new Icon(Spr, null, Colors.textMain);
	}
	
	public override List<Tooltip> GetTooltips(State s)
	{
		if (s.route is Combat)
		{
			return
			[
				new GlossaryTooltip($"MaginotActionMaxShieldHintHand")
				{
					Description = string.Format(ModEntry.Instance.Localizations.Localize(["action", "MaxShieldHint", "description.hand"]), s.ship.GetMaxShield())
				},
			];
		}
		else
		{
			return
			[
				new GlossaryTooltip($"MaginotActionMaxShieldHintDeck")
				{
					Description = ModEntry.Instance.Localizations.Localize(["action", "MaxShieldHint", "description.deck"])
				},
			];
		}
	}
}