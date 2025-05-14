using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardScareTactics : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(new CardConfiguration
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new CardMeta
			{
				deck = ModEntry.Instance.MaginotDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ScareTactics", "name"]).Localize,
			Art = ModEntry.RegisterSprite(package, "assets/Card/Flench.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			exhaust = true,
			retain = upgrade == Upgrade.A
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new AStatus
			{
				status = Status.autododgeLeft,
				statusAmount = 1,
				targetPlayer = false
			},
			new MaginotActionArtilleryAttack
			{
				damage = GetDmg(s, upgrade == Upgrade.B ? 16 : 8)
			}
		];
	}
}
