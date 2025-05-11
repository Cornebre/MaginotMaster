using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Cornebre.Maginot.Actions;
//using Cornebre.Maginot.Artifacts;
using Cornebre.Maginot.Cards;
using Cornebre.Maginot.External;
using Cornebre.Maginot.Features;
//using TheJazMaster.MoreDifficulties;

namespace Cornebre.Maginot;

internal class ModEntry : SimpleMod
{
	internal static ModEntry Instance { get; private set; } = null!;
	internal Harmony Harmony;
	internal IKokoroApi.IV2 KokoroApi;
	internal IDeckEntry MaginotDeck;
	internal IStatusEntry MaginotManagerArtilleryBank;
	internal IStatusEntry MaginotManagerActiveShielding;
	internal IStatusEntry MaginotManagerAutoShield;
	internal IStatusEntry MaginotManagerAutoTempShield;
	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi {get; private set; } = null!;

	private static readonly List<Type> MaginotCommonCardTypes = [
		typeof(MaginotCardArtilleryShot),
		typeof(MaginotCardBrace),
		typeof(MaginotCardBuildUp),
		typeof(MaginotCardFrontLine),
		typeof(MaginotCardHammerThrough),
		typeof(MaginotCardHeavyCaltrops),
		typeof(MaginotCardHoldTheLine),
		typeof(MaginotCardMobileFort),
		typeof(MaginotCardPowerToShields),
	];
	private static readonly List<Type> MaginotUncommonCardTypes = [
		typeof(MaginotCardBunkerDown),
		typeof(MaginotCardCoverShot),
		typeof(MaginotCardECM),
		typeof(MaginotCardActiveShielding),
		typeof(MaginotCardPavis),
		typeof(MaginotCardSupplyChain),
		typeof(MaginotCardTwinArtillery),
	];
	private static readonly List<Type> MaginotRareCardTypes = [
		typeof (MaginotCardAllOrBurst),
		typeof (MaginotCardArtilleryBank),
		typeof (MaginotCardArtilleryBarrage),
		typeof (MaginotCardNastySurprise),
		typeof (MaginotCardScareTactics),
	];
	private static readonly List<Type> MaginotSpecialCardTypes = [
		typeof (MAginotCardArtilleryShell),
	];
	private static readonly IEnumerable<Type> MaginotCardTypes =
		MaginotCommonCardTypes
			.Concat(MaginotUncommonCardTypes)
			.Concat(MaginotRareCardTypes)
			.Concat(MaginotSpecialCardTypes);

	private static readonly List<Type> MaginotCommonArtifacts = [
	];
	private static readonly List<Type> MaginotBossArtifacts = [
	];
	private static readonly IEnumerable<Type> MaginotArtifactTypes =
		MaginotCommonArtifacts
			.Concat(MaginotBossArtifacts);

	private static readonly IEnumerable<Type> AllRegisterableTypes =
		MaginotCardTypes
			.Concat(MaginotArtifactTypes);

	public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new Harmony(package.Manifest.UniqueName);
		Harmony.PatchAll();
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		MaginotDeck = helper.Content.Decks.RegisterDeck("Maginot", new DeckConfiguration
		{
			Definition = new DeckDef
			{
				color = new Color("d5c58a"),

				titleColor = new Color("000000")
			},

			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = RegisterSprite(package, "assets/frame_maginot.png").Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

		foreach (var type in AllRegisterableTypes)
			AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
		
		/*
		 * Characters have required animations, recommended animations, and you have the option to add more.
		 * In addition, they must be registered before the character themselves is registered.
		 * The game requires you to have a neutral animation and mini animation, used for normal gameplay and the map and run start screen, respectively.
		 * The game uses the squint animation for the Extra-Planar Being and High-Pitched Static events, and the gameover animation while you are dying.
		 * You may define any other animations, and they will only be used when explicitly referenced (such as dialogue).
		 */
		RegisterAnimation(package, "neutral", "assets/Animation/DaveNeutral", 4);
		RegisterAnimation(package, "squint", "assets/Animation/DaveSquint", 4);
		Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
		{
			CharacterType = MaginotDeck.Deck.Key(),
			LoopTag = "gameover",
			Frames = [
				RegisterSprite(package, "assets/Animation/DaveGameOver.png").Sprite,
			]
		});
		Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
		{
			CharacterType = MaginotDeck.Deck.Key(),
			LoopTag = "mini",
			Frames = [
				RegisterSprite(package, "assets/Animation/DaveMini.png").Sprite,
			]
		});

		helper.Content.Characters.V2.RegisterPlayableCharacter("Maginot", new PlayableCharacterConfigurationV2
		{
			Deck = MaginotDeck.Deck,
			BorderSprite = RegisterSprite(package, "assets/char_frame_maginot.png").Sprite,
			Starters = new StarterDeck
			{
				cards = [
					new MaginotCardBuildUp(),
					new MaginotCardHeavyCaltrops()
				],
				artifacts = [
				]
			},
			Description = AnyLocalizations.Bind(["character", "desc"]).Localize
		});

		helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>(
			"TheJazMaster.MoreDifficulties",
			api => api.RegisterAltStarters(
				deck: MaginotDeck.Deck,
				starterDeck: new StarterDeck
				{
					cards = [
						new MaginotCardArtilleryShot(),
						new MaginotCardFrontLine()
					],
					artifacts = [
					]
				}
			)
		);

		MaginotManagerArtilleryBank = helper.Content.Statuses.RegisterStatus("MaginotManagerArtilleryBank", new StatusConfiguration
		{
			Definition = new StatusDef
			{
				isGood = true,
				affectedByTimestop = false,
				color = new Color("d5c58a"),
				icon = RegisterSprite(package, "assets/Icons/artilleryBank.png").Sprite
			},
			Name = AnyLocalizations.Bind(["status", "artilleryBank", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "artilleryBank", "desc"]).Localize
		});
		MaginotManagerActiveShielding = helper.Content.Statuses.RegisterStatus("MaginotManagerActiveShielding", new StatusConfiguration
		{
			Definition = new StatusDef
			{
				isGood = true,
				affectedByTimestop = true,
				color = new Color("d5c58a"),
				icon = RegisterSprite(package, "assets/Icons/armoredStatus.png").Sprite
			},
			Name = AnyLocalizations.Bind(["status", "activeShielding", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "activeShielding", "desc"]).Localize
		});
		MaginotManagerAutoShield = helper.Content.Statuses.RegisterStatus("MaginotManagerAutoShield", new StatusConfiguration
		{
			Definition = new StatusDef
			{
				isGood = true,
				affectedByTimestop = false,
				color = new Color("d5c58a"),
				icon = RegisterSprite(package, "assets/Icons/autoShield.png").Sprite
			},
			Name = AnyLocalizations.Bind(["status", "autoShield", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "autoShield", "desc"]).Localize
		});
		MaginotManagerAutoTempShield = helper.Content.Statuses.RegisterStatus("MaginotManagerAutoTempShield+", new StatusConfiguration
		{
			Definition = new StatusDef
			{
				isGood = true,
				affectedByTimestop = false,
				color = new Color("d5c58a"),
				icon = RegisterSprite(package, "assets/Icons/autoTempShield.png").Sprite
			},
			Name = AnyLocalizations.Bind(["status", "autoTempShield", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "autoTempShield", "desc"]).Localize
		});
		
		_ = new MaginotManagerArtilleryBank(package, helper);
		_ = new MaginotManagerActiveShielding(package, helper);
		_ = new MaginotManagerAutoShield(package, helper);
		_ = new MaginotManagerAutoTempShield(package, helper);

		/*
		 * Some classes require so little management that a manager may not be worth writing.
		 */
		MaginotActionNastySurprise.Spr = RegisterSprite(package, "assets/Icons/nastySurprise.png").Sprite;
		MaginotActionArtilleryAttack.Spr = RegisterSprite(package, "assets/Icons/artilleryAttack.png").Sprite;
		MaginotActionMaxShieldHint.Spr = RegisterSprite(package, "assets/Icons/totalShield.png").Sprite;
	}

	/*
	 * assets must also be registered before they may be used.
	 * Unlike cards and artifacts, however, they are very simple to register, and often do not need to be referenced in more than one place.
	 * This utility method exists to easily register a sprite, but nothing prevents you from calling the method used yourself.
	 */
	public static ISpriteEntry RegisterSprite(IPluginPackage<IModManifest> package, string dir)
	{
		return Instance.Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile(dir));
	}

	/*
	 * Animation frames are typically named very similarly, only differing by the number of the frame itself.
	 * This utility method exists to easily register an animation.
	 * It expects the animation to start at frame 0, up to frames - 1.
	 * TODO It is advised to avoid animations consisting of 2 or 3 frames.
	 */
	public static void RegisterAnimation(IPluginPackage<IModManifest> package, string tag, string dir, int frames)
	{
		Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
		{
			CharacterType = Instance.MaginotDeck.Deck.Key(),
			LoopTag = tag,
			Frames = Enumerable.Range(0, frames)
				.Select(i => RegisterSprite(package, dir + i + ".png").Sprite)
				.ToImmutableList()
		});
	}
}

