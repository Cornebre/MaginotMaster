using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardECM : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ECM", "name"]).Localize,
			Art = ModEntry.Instance.FlenchEMP
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			exhaust = upgrade != Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new AStatus
			{
				status = Status.backwardsMissiles,
				statusAmount = upgrade == Upgrade.A ? 4 : 2,
				targetPlayer = false
			}
		];
	}
}
