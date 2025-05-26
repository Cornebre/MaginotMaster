using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardBuildUp : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BuildUp", "name"]).Localize,
			Art = ModEntry.Instance.FlenchBuildUp
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
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
					status = Status.maxShield,
					statusAmount = 1,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 2,
					targetPlayer = true
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 2)
				}
			],
			Upgrade.B => [
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 2,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 3,
					targetPlayer = true
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 2)
				}
			],
			_ => [
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 1,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 1)
				}
			]
		};
	}
}
