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
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				},
				new AVariableHint
				{
					status = Status.shield
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetValueAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new AVariableHint
				{
					status = Status.shield,
					secondStatus = Status.evade
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetValueAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				},
				new AStatus
				{
					status = Status.evade,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				}
			],
			_ => [
				new AVariableHint
				{
					status = Status.shield
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, GetValueAmt(s) * 2),
					xHint = 2
				},
				new AStatus
				{
					status = Status.shield,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				}
			]
		};
	}
	
	private int GetValueAmt(State s)
	{
		int num = 0;
		if (s.route is Combat)
		{
			num = s.ship.Get(Status.shield);
			if (upgrade == Upgrade.B)
			{
				num += s.ship.Get(Status.tempShield);
			}
			if (upgrade == Upgrade.A)
			{
				num++;
				if (num > s.ship.GetMaxShield())
				{
					num = s.ship.GetMaxShield();
				}
			}
		}
		return num;
	}
}
