using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardSupplyChain : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SupplyChain", "name"]).Localize,
			Art = ModEntry.Instance.FlenchBunkerDown
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
			Upgrade.B => [
				new AStatus
				{
					status = ModEntry.Instance.MaginotManagerAutoShield.Status,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			_ => [
				new AStatus
				{
					status = ModEntry.Instance.MaginotManagerAutoTempShield.Status,
					statusAmount = upgrade == Upgrade.A ? 3 : 2,
					targetPlayer = true
				}
			]
		};
	}
}
