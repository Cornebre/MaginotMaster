using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardPavis : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(new CardConfiguration
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new CardMeta
			{
				deck = ModEntry.Instance.MaginotDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Pavis", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 3,
			description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "Pavis", "description"]), GetPavisTotal(state)),
			exhaust = upgrade != Upgrade.A,
			retain = upgrade == Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new AStatus
			{
				status = Status.tempShield,
				statusAmount = GetPavisTotal(s),
				targetPlayer = true
			}
		];
	}
	private int GetPavisTotal(State s)
	{
		int num = 0;
		if (s.route is Combat combat)
		{
			foreach (Part part in combat.otherShip.parts)
			{
				if (part.intent is IntentAttack intentAttack)
				{
					num += GetDmg(s, intentAttack.damage, targetPlayer: true) * intentAttack.multiHit;
				}
			}
		}
		return num;
	}
}
