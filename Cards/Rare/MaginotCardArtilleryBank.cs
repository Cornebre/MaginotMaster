using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardArtilleryBank : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ArtilleryBank", "name"]).Localize,
			Art = ModEntry.Instance.FlenchArtilleryBank
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			exhaust = upgrade != Upgrade.A
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.B => [
				new AStatus
				{
					status = ModEntry.Instance.MaginotManagerArtilleryBank.Status,
					statusAmount = 1,
					targetPlayer = true
				},
				new MaginotActionArtilleryAttack
				{
					damage = GetDmg(s, 2 )
				}
			],
			_ => [
				new AStatus
				{
					status = ModEntry.Instance.MaginotManagerArtilleryBank.Status,
					statusAmount = 1,
					targetPlayer = true
				}
			]
		};
	}
}
