namespace Cornebre.Maginot;

public static class Extensions {
	public static AAttack ApplyArtillery(this AAttack attack, bool artillery) {
		if (!artillery) return attack;

		ModEntry.Instance.Helper.ModData.SetModData(attack, ArtilleryShotController.key, artillery);
		return attack;
	}
}