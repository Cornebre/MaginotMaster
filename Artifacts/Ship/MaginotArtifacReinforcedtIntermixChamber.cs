using System.Reflection;
using Nanoray.PluginManager;
using System.Linq;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactReinforcedIntermixChamber : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new ArtifactMeta
			{
				pools = [ArtifactPool.Boss],
				owner = Deck.colorless,
				unremovable = true
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReinforcedIntermixChamber", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReinforcedIntermixChamber", "desc"]).Localize,
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/ReinforcedIntermixChamber.png")).Sprite
		});
	}
	
	public override void OnReceiveArtifact(State state)
	{
		state.ship.baseEnergy++;
		state.GetCurrentQueue().QueueImmediate(new ALoseArtifact { artifactType = new MaginotArtifactIntermixChamber().Key() });
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseEnergy--;
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		combat.QueueImmediate(from pair in state.ship.parts.Select((Part part, int x) => new { part, x })
			where pair.part.type == PType.wing
			select new AWeaken
			{
				targetPlayer = true,
				worldX = state.ship.x + pair.x,
				justTheActiveOverride = true,
				artifactPulse = Key()
			});
	}
}