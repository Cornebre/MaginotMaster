using System;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardBrace : Card, IRegisterable
{
	public bool played;

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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Brace", "name"]).Localize
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 0,
			floppable = true,
			infinite = upgrade == Upgrade.A && !flipped,
			retain = upgrade == Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		if (flipped)
		{
			return [
				new AStatus
				{
					status = Status.tempShield,
					statusAmount = upgrade == Upgrade.B ? 3 : 2,
					disabled = true,
					targetPlayer = true
				},
				new ADummyAction(),
				new ADrawCard
				{
					count = played ? 0 : 1,
					disabled = false
				}
			];
		}
		else
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
						disabled = flipped,
						targetPlayer = true
					}
				).AsCardAction,
				new ADummyAction(),
				new ADrawCard
				{
					count = played ? 0 : 1,
					disabled = true
				}
			];
		}
	}

	public override void OnDraw(State s, Combat c)
	{
		played = false;
	}

	public override void AfterWasPlayed(State state, Combat c)
	{
		played = true;
	}
}
