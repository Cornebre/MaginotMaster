using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardMobileFort : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MobileFort", "name"]).Localize,
			Art = ModEntry.Instance.FlenchCaltrops
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.A ? 1 : 2
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new ASpawn
			{
				thing = new Asteroid
				{
					yAnimation = 0.0,
					bubbleShield = upgrade == Upgrade.B
				}
			},
			new AStatus
			{
				status = Status.droneShift,
				statusAmount = 2,
				targetPlayer = true
			}
		];
	}
}

