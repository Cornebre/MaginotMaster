using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Cornebre.Maginot.Actions;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardNastySurprise : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NastySurprise", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.A ? 1 : 2,
			retain = upgrade == Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new MaginotActionNastySurprise
			{
				Count = 1
			},
			new AStatus
			{
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			}
		];
	}
}
