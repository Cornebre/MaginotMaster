using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Cards;

internal sealed class MaginotCardTwinArtillery : Card, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TwinArtillery", "name"]).Localize,
			Art = ModEntry.RegisterSprite(package, "assets/Card/Flench.png").Sprite
		});
	}

	public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = upgrade == Upgrade.B ? 4 : 3
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return [
			new MaginotActionArtilleryAttack
			{
				damage = GetDmg(s, upgrade == Upgrade.A ? 4 : upgrade == Upgrade.B ? 5 : 3 )
			},
			new MaginotActionArtilleryAttack
			{
				damage = GetDmg(s, upgrade == Upgrade.A ? 4 : upgrade == Upgrade.B ? 5 : 3 )
			}
		];
	}
}
