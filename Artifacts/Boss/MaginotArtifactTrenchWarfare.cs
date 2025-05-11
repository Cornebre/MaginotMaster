using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot.Artifacts;

public class MaginotArtifactTrenchWarfare : Artifact, IRegisterable
{
	private const int SideLength = 30;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact(new ArtifactConfiguration
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new ArtifactMeta
			{
				pools = [ArtifactPool.Boss],
				owner = ModEntry.Instance.MaginotDeck.Deck
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TrenchWarfare", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TrenchWarfare", "desc"]).Localize,
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/TrenchWarfare.png")).Sprite
		});
	}
	
	public override void OnCombatStart(State state, Combat combat)
	{
		List<int> list = new List<int>();
		for (int i = state.ship.x - SideLength; i < state.ship.x + state.ship.parts.Count() + SideLength; i++)
		{
			if (!combat.stuff.ContainsKey(i))
			{
				list.Add(i);
			}
		}
		foreach (int item in list)
		{
			combat.stuff.Add(item, new Asteroid
			{
				targetPlayer = false,
				x = item,
				xLerped = item
			});
		}
		Pulse();
	}
	
	public override List<Tooltip>? GetExtraTooltips()
	{
		return
		[
			new TTGlossary("midrow.asteroid")
		];
	}

}