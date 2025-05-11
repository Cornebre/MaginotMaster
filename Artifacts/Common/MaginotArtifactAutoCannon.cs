using System.Collections.Generic;
using System.Reflection;
using Cornebre.Maginot.Cards;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactAutoCannon : Artifact, IRegisterable
{
	public int count;
	private const int TRIGGER_POINT = 6;
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
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "AutoCannon", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "AutoCannon", "desc"]).Localize,
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/AutoCannon.png")).Sprite
		});
	}
	
	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (status == Status.shield && mode == AStatusMode.Add && statusAmount > 0)
		{
			count++;
			Pulse();
		}
		if (count >= TRIGGER_POINT)
		{
			combat.QueueImmediate(new AAddCard {
				card = new MAginotCardArtilleryShell(),
				amount = 1,
				destination = CardDestination.Hand,
				artifactPulse = Key()
			});
			count = 0;
		}
	}
	
	public override int? GetDisplayNumber(State s)
	{
		return count;
	}
	
	public override List<Tooltip>? GetExtraTooltips()
	{
		List<Tooltip> list = [new TTCard {card = new MAginotCardArtilleryShell()}];
		return list;
	}
}