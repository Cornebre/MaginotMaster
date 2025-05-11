using System.Collections.Generic;
using Nickel;

namespace Cornebre.Maginot.Actions;

public class MaginotActionArtilleryAttack : AAttack
{
	
	public static Spr Spr;
	StuffBase? stuffBase = null;
	public override void Begin(G g, State s, Combat c)
	{
		RemoveMidrow(s,c);
		base.Begin(g, s, c);
		RestoreMidrow(s,c);
	}
	
	public override Icon? GetIcon(State s)
	{
		Color FinalColour = Colors.redd;
		if (!DoWeHaveCannonsThough(s))
		{
			FinalColour = Colors.attackFail;
		}
		return new Icon
		{
			path = Spr,
			number = damage,
			color = FinalColour
		};
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		base.GetTooltips(s);
		return
		[
			new GlossaryTooltip($"MaginotActionArtilleryAttack")
			{
				Icon = Spr,
				Title = ModEntry.Instance.Localizations.Localize(["action", "ArtilleryAttack", "title"]),
				TitleColor = Colors.card,
				Description = ModEntry.Instance.Localizations.Localize(["action", "ArtilleryAttack", "description"])
			},
		];
	}
	private void RemoveMidrow(State s, Combat c)
	{
		Ship ship = !targetPlayer ? s.ship : c.otherShip;
		if (ship == null || ship.hull <= 0 || (fromDroneX.HasValue && !c.stuff.ContainsKey(fromDroneX.Value)))
		{
			return;
		}
		int? num = GetFromX(s,c);
		if (num.HasValue)
		{
			num += ship.x;
			if (c.stuff.TryGetValue(num.Value, out StuffBase? value))
			{
				stuffBase = Mutil.DeepCopy(value);
				c.stuff.Remove(num.Value);
			}
		}
	}
	private void RestoreMidrow(State s, Combat c)
	{
		int? num = GetFromX(s,c);
		if (num.HasValue && stuffBase != null)
		{
			c.stuff[num.Value] = stuffBase;
		}
	}
}