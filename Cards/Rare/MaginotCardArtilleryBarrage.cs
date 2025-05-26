﻿using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardArtilleryBarrage : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ArtilleryBarrage", "name"]).Localize,
			Art = ModEntry.Instance.FlenchArtilleryBank
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 2,
			description = ModEntry.Instance.Localizations.Localize(["card", "ArtilleryBarrage", "description", upgrade.ToString()]),
			exhaust = true
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.B => [
				new AAddCard
				{
					card = new MAginotCardArtilleryShell()
					{
						upgrade = Upgrade.A
					},
					amount = 3,
					destination = CardDestination.Hand
				},
				new AAddCard
				{
					card = new MAginotCardArtilleryShell()
					{
						upgrade = Upgrade.A
					},
					amount = 2,
					destination = CardDestination.Deck,
					omitFromTooltips = true
				}
			],
			_ => [
				new AAddCard
				{
					card = new MAginotCardArtilleryShell()
					{
						upgrade = Upgrade.A
					},
					amount = upgrade == Upgrade.A ? 4 : 3,
					destination = CardDestination.Hand
				},
			]
		};
	}
}
