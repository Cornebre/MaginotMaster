using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardActiveShielding : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ActiveShielding", "name"]).Localize,
			Art = ModEntry.RegisterSprite(package, "assets/Card/Flench.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.None ? 1 : 0,
			recycle = upgrade == Upgrade.B,
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new AStatus
			{
				status = ModEntry.Instance.MaginotManagerActiveShielding.Status,
				statusAmount = upgrade == Upgrade.B ? 1 : 2,
				targetPlayer = true
			}
		];
	}
}
