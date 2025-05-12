using System.Collections.Generic;
using System.Linq;
using Nickel;

namespace Cornebre.Maginot.Actions;

public class MaginotActionNastySurprise : CardAction
{
	public static Spr Spr;
	
	public int Count;

	public override void Begin(G g, State s, Combat c)
	{
		foreach (StuffBase item in c.stuff.Values.ToList())
		{
			if (item is not SpaceMine)
			{
				c.stuff.Remove(item.x);
				SpaceMine value = new SpaceMine
				{
					x = item.x,
					xLerped = item.xLerped,
					bubbleShield = item.bubbleShield,
					targetPlayer = item.targetPlayer,
					age = item.age
				};
				c.stuff[item.x] = value;
			}
		}
	}

	public override Icon? GetIcon(State s)
	{
		return new Icon
		{
			path = Spr,
			number = Count,
			color = Colors.textMain
		};
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		return
		[
			new GlossaryTooltip($"MaginotActioNastySurprise")
			{
				Icon = Spr,
				Title = ModEntry.Instance.Localizations.Localize(["action", "NastySurprise", "title"]),
				TitleColor = Colors.card,
				Description = ModEntry.Instance.Localizations.Localize(["action", "NastySurprise", "description"])
			},
		];
	}
}