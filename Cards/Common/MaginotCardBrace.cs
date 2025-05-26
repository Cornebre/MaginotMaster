using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardBrace : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Brace", "name"]).Localize,
			Art = ModEntry.Instance.FlenchShield
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 0,
			infinite = upgrade == Upgrade.A,
			retain = upgrade == Upgrade.B,
			unplayable = upgrade == Upgrade.A && state.ship.Get(Status.shield) == 0,
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(
				ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(
					ModEntry.Instance.KokoroApi.ActionCosts.MakeStatusResource(Status.shield),
					1
				),
				new AStatus
				{
					status = Status.tempShield,
					statusAmount = upgrade == Upgrade.B ? 3 : 2,
					targetPlayer = true
				}
			).AsCardAction,
		];
	}
}
