using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardHammerThrough : Card, IRegisterable
{
	public int value = 1;
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HammerThrough", "name"]).Localize,
			// Art = ModEntry.RegisterSprite(package, "assets/Card/Illeana/1/Autotomy.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 1,
			description = string.Format(ModEntry.Instance.Localizations.Localize(["card", "HammerThrough", "description", upgrade.ToString()]), value)
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new ADrawCard
				{
					count = 1
				},
				new AAttack
				{
					damage = GetDmg(s, value)
				}
			],
			Upgrade.B => [
				new AAttack
				{
					damage = GetDmg(s, value)
				}
			],
			_ => [
				new AAttack
				{
					damage = GetDmg(s, value)
				}
			]
		};
	}
	
	public override void OnExitCombat(State s, Combat c)
	{
		value = 1;
	}

	public override void AfterWasPlayed(State state, Combat c)
	{
		value++;
		if (upgrade == Upgrade.B)
		{
			value++;
		}
	}
}
