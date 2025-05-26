using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardCoverShot : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CoverShot", "name"]).Localize,
			Art = ModEntry.Instance.FlenchBuildUp
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
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 2),
				},
				new ASpawn
				{
					thing = new AttackDrone
					{
						yAnimation = 0.0,
					},
					offset = 1
				}
			],
			_ => [
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 2),
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0,
						bubbleShield = upgrade == Upgrade.A
					},
					offset = 1
				}
			]
		};
	}
}

