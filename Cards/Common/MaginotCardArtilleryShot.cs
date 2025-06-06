﻿using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardArtilleryShot : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ArtilleryShot", "name"]).Localize,
			Art = ModEntry.Instance.FlenchArtillery
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			exhaust = upgrade == Upgrade.B
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new MaginotActionArtilleryAttack
			{
				damage = GetDmg(s, upgrade == Upgrade.A ? 5 : upgrade == Upgrade.B ? 7 : 3)
			}
		];
	}
}
