using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactMomentumCollector : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new ArtifactMeta
			{
				pools = [ArtifactPool.Common],
				owner = ModEntry.Instance.MaginotDeck.Deck
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MomentumCollector", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MomentumCollector", "desc"]).Localize,
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/MomentumCollector.png")).Sprite
		});
	}
	
	public override int ModifySpaceMineDamage(State state, Combat? combat, bool targetPlayer)
	{
		combat?.QueueImmediate(
		[
			new AStatus
			{
				status = Status.droneShift,
				targetPlayer = true,
				statusAmount = 1,
				artifactPulse = Key()
			}
		]);
		return 0;
	}
}