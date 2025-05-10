using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardBunkerDown : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BunkerDown", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 1,
			exhaust = true
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 5,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 3,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.tempShield,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			_ => [
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 3,
					targetPlayer = true
				}
			]
		};
	}
}
