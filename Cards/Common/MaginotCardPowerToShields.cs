using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardPowerToShields : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(new CardConfiguration
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new CardMeta
			{
				deck = ModEntry.Instance.MaginotDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PowerToShields", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.B ? 1 : 3,
			exhaust = upgrade == Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AStatus
				{
					status = Status.tempShield,
					statusAmount = 3,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 5,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new AStatus
				{
					status = Status.shield,
					statusAmount = 5,
					targetPlayer = true
				}
			],
			_ => [
				new AStatus
				{
					status = Status.shield,
					statusAmount = 5,
					targetPlayer = true
				}
			]
		};
	}
}
