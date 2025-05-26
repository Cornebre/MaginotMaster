using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardHeavyCaltrops : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HeavyCaltrops", "name"]).Localize,
			Art = ModEntry.Instance.FlenchCaltrops
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			flippable = true
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.B => [
				new ASpawn
				{
					thing = new SpaceMine
					{
						yAnimation = 0.0
					},
					offset = 1
				},
				new ASpawn
				{
					thing = new SpaceMine
					{
						yAnimation = 0.0
					},
				},
				new AStatus
				{
					status = Status.droneShift,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			_ => [
				new ASpawn
				{
					thing = new SpaceMine
					{
						yAnimation = 0.0,
						bigMine = upgrade == Upgrade.A
					},
					offset = 1
				},
				new ASpawn
				{
					thing = new SpaceMine
					{
						yAnimation = 0.0,
						bigMine = upgrade == Upgrade.A
					},
				}
			]
		};
	}
}
