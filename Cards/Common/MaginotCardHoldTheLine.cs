using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardHoldTheLine : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HoldTheLine", "name"]).Localize,
			Art = ModEntry.Instance.FlenchBuildUp
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.B ? 3 : 2
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AStatus
				{
					status = Status.tempPayback,
					statusAmount = 1,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new AStatus
				{
					status = Status.tempPayback,
					statusAmount = 2,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			_ => [
				new AStatus
				{
					status = Status.tempPayback,
					statusAmount = 1,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				}
			]
		};
	}
}
