using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardMaginotEXE : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(new CardConfiguration
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new CardMeta
			{
				deck = Deck.colorless,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MaginotEXE", "name"]).Localize,
			Art = ModEntry.Instance.FlenchBase
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.MaginotDeck.Deck),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		Deck deck = ModEntry.Instance.MaginotDeck.Deck;
		return upgrade switch
		{
			Upgrade.B => [
				new ACardOffering
				{
					amount = 3,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
				}
			],
			_ => [
				new ACardOffering
				{
					amount = 2,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
				}
			],
		};
	}
}