using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactMedalOfHonor : Artifact, IRegisterable
{
	private const int BASE_VALUE = 3;
	public int count = BASE_VALUE;
	private static Spr activeSprite;
	private static Spr inactiveSprite;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		activeSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/MedalOfHonorActive.png")).Sprite;
		inactiveSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/MedalOfHonorInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new ArtifactMeta
			{
				pools = [ArtifactPool.Common],
				owner = ModEntry.Instance.MaginotDeck.Deck
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MedalOfHonor", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MedalOfHonor", "desc"]).Localize,
			Sprite = activeSprite
		});
	}
	
	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (status == Status.shield && statusAmount > 0 && state.ship.Get(Status.shield) == state.ship.GetMaxShield() && count >= 0)
		{
			count--;
			combat.QueueImmediate(new ADrawCard
			{
				count = 1,
				artifactPulse = Key()
			});
		}
	}

	public override int? GetDisplayNumber(State s)
	{
		return count;
	}
	public override void OnTurnEnd(State state, Combat combat)
	{
		count = BASE_VALUE;
	}
	public override void OnCombatEnd(State state)
	{
		count = BASE_VALUE;
	}
	public override Spr GetSprite()
	{
		if (count >= 0)
		{
			return activeSprite;
		}
		return inactiveSprite;
	}
}