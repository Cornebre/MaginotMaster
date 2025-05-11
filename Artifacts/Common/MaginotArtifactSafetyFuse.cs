using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactSafetyFuse : Artifact, IRegisterable
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SafetyFuse", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SafetyFuse", "desc"]).Localize,
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/SafetyFuse.png")).Sprite
		});
	}
	
	public override int ModifySpaceMineDamage(State state, Combat? combat, bool targetPlayer)
	{
		return targetPlayer ? -99 : 0;
	}

}