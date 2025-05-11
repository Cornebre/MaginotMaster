using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardAllOrBurst : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "AllOrBurst", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 3,
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
					statusAmount = 1,
					targetPlayer = true
				},
				new MaginotActionMaxShieldHint
				{
					status = Status.maxShield
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetMaxShieldAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = - s.ship.GetMaxShield(),
					xHint = -1,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new MaginotActionMaxShieldHint
				{
					status = Status.maxShield
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetMaxShieldAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				}
			],
			_ => [
				new MaginotActionMaxShieldHint
				{
					status = Status.maxShield
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetMaxShieldAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.maxShield,
					statusAmount = - s.ship.GetMaxShield(),
					xHint = -1,
					targetPlayer = true
				}
			]
		};
	}
	
	private int GetMaxShieldAmt(State s)
	{
		int result = 0;
		if (s.route is Combat)
		{
			result = s.ship.GetMaxShield();
		}
		return result;
	}
}
