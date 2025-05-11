using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactMilitaryDiscount : Artifact, IRegisterable
{
	public bool alreadyActivated;
	private static Spr activeSprite;
	private static Spr inactiveSprite;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		activeSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/MilitaryDiscountActive.png")).Sprite;
		inactiveSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/MilitaryDiscountInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new ArtifactMeta
			{
				pools = [ArtifactPool.Boss],
				owner = ModEntry.Instance.MaginotDeck.Deck
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MilitaryDiscount", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MilitaryDiscount", "desc"]).Localize,
			Sprite = activeSprite
		});
	}
	
	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (energyCost >= 2 && deck == ModEntry.Instance.MaginotDeck.Deck && !alreadyActivated)
		{
			combat.QueueImmediate(new AEnergy
			{
				changeAmount = 1,
				artifactPulse = Key()
			});
			alreadyActivated = true;
		}
	}
	
	public override void OnTurnEnd(State state, Combat combat)
	{
		alreadyActivated = false;
	}

	public override void OnCombatEnd(State state)
	{
		alreadyActivated = false;
	}
	public override Spr GetSprite()
	{
		if (!alreadyActivated)
		{
			return activeSprite;
		}
		return inactiveSprite;
	}
}