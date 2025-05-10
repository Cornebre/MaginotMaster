using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardFrontLine : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FrontLine", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 1,
			flippable = true
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0,
						bubbleShield = true
					},
					offset = -1
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0,
						bubbleShield = true
					},
				}
			],
			Upgrade.B => [
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					},
					offset = -3
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					},
					offset = -2
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					},
					offset = -1
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					}
				}
			],
			_ => [
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					},
					offset = -1
				},
				new ASpawn
				{
					thing = new Asteroid
					{
						yAnimation = 0.0
					},
				}
			]
		};
	}
}
