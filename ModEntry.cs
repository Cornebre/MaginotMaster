﻿using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Cornebre.Maginot.Actions;
using Cornebre.Maginot.Artifacts;
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
	internal IShipEntry Maginot_Ship { get; }
	internal IStatusEntry MaginotManagerArtilleryBank;
	internal IStatusEntry MaginotManagerActiveShielding;
	internal IStatusEntry MaginotManagerAutoShield;
	internal IStatusEntry MaginotManagerAutoTempShield;
	internal Spr? FlenchBase;
	internal Spr? FlenchArtillery;
	internal Spr? FlenchCaltrops;
	internal Spr? FlenchTwinArtillery;
	internal Spr? FlenchBuildUp;
	internal Spr? FlenchBunkerDown;
	internal Spr? FlenchArtilleryBank;
	internal Spr? FlenchAllOrBurst;
	internal Spr? FlenchScare;
	internal Spr? FlenchShield;
	internal Spr? FlenchEMP;


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
		typeof(MaginotCardAllOrBurst),
		typeof(MaginotCardArtilleryBank),
		typeof(MaginotCardArtilleryBarrage),
		typeof(MaginotCardNastySurprise),
		typeof(MaginotCardScareTactics),
	];
	private static readonly List<Type> MaginotSpecialCardTypes = [
		typeof(MAginotCardArtilleryShell),
		typeof(MaginotCardMaginotEXE),
	];
	private static readonly IEnumerable<Type> MaginotCardTypes =
		MaginotCommonCardTypes
			.Concat(MaginotUncommonCardTypes)
			.Concat(MaginotRareCardTypes)
			.Concat(MaginotSpecialCardTypes);

	private static readonly List<Type> MaginotCommonArtifacts = [
		typeof(MaginotArtifactSafetyFuse),
		typeof(MaginotArtifactAutoCannon),
		typeof(MaginotArtifactMomentumCollector),
		typeof(MaginotArtifactMedalOfHonor),
	];
	private static readonly List<Type> MaginotBossArtifacts = [
		typeof(MaginotArtifactActiveShielding),
		typeof(MaginotArtifactMilitaryDiscount),
		typeof(MaginotArtifactTrenchWarfare),
	];
	private static readonly List<Type> MaginotSpecialArtifacts = [
		typeof(MaginotArtifactIntermixChamber),
		typeof(MaginotArtifactReinforcedIntermixChamber),
	];
	private static readonly IEnumerable<Type> MaginotArtifactTypes =
		MaginotCommonArtifacts
			.Concat(MaginotBossArtifacts)
			.Concat(MaginotSpecialArtifacts);

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

		FlenchBase = RegisterSprite(package, "assets/Card/FlenchBase.png").Sprite;
		FlenchArtillery = RegisterSprite(package, "assets/Card/FlenchArtillery.png").Sprite;
		FlenchCaltrops = RegisterSprite(package, "assets/Card/FlenchCaltrops.png").Sprite;
		FlenchTwinArtillery = RegisterSprite(package, "assets/Card/FlenchTwinArtillery.png").Sprite;
		FlenchBuildUp = RegisterSprite(package, "assets/Card/FlenchBuildUp.png").Sprite;
		FlenchBunkerDown = RegisterSprite(package, "assets/Card/FlenchBunkerDown.png").Sprite;
		FlenchArtilleryBank = RegisterSprite(package, "assets/Card/FlenchArtilleryBank.png").Sprite;
		FlenchAllOrBurst = RegisterSprite(package, "assets/Card/FlenchAllOrBurst.png").Sprite;
		FlenchScare = RegisterSprite(package, "assets/Card/FlenchScare.png").Sprite;
		FlenchShield = RegisterSprite(package, "assets/Card/FlenchShield.png").Sprite;
		FlenchEMP = RegisterSprite(package, "assets/Card/FlenchEMP.png").Sprite;
		foreach (var type in AllRegisterableTypes)
			AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
		
		/*
		 * Characters have required animations, recommended animations, and you have the option to add more.
		 * In addition, they must be registered before the character themselves is registered.
		 * The game requires you to have a neutral animation and mini animation, used for normal gameplay and the map and run start screen, respectively.
		 * The game uses the squint animation for the Extra-Planar Being and High-Pitched Static events, and the gameover animation while you are dying.
		 * You may define any other animations, and they will only be used when explicitly referenced (such as dialogue).
		 */
		RegisterAnimation(package, "excited", "assets/Animation/maginot_excited_", 4);
		RegisterAnimation(package, "frown", "assets/Animation/maginot_frown_", 4);
		RegisterAnimation(package, "neutral", "assets/Animation/maginot_neutral_", 4);
		RegisterAnimation(package, "reminicent", "assets/Animation/maginot_reminicent_", 4);
		RegisterAnimation(package, "salute", "assets/Animation/maginot_salute_", 4);
		RegisterAnimation(package, "squint", "assets/Animation/maginot_squint_", 4);
		Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
		{
			CharacterType = MaginotDeck.Deck.Key(),
			LoopTag = "gameover",
			Frames = [
				RegisterSprite(package, "assets/Animation/maginot_gameover.png").Sprite,
			]
		});
		Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
		{
			CharacterType = MaginotDeck.Deck.Key(),
			LoopTag = "mini",
			Frames = [
				RegisterSprite(package, "assets/Animation/maginot_mini.png").Sprite,
			]
		});

		var maginotShipPartWing = helper.Content.Ships.RegisterPart("maginotPart.Wing", new PartConfiguration()
		{
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/wing_excelsior.png")).Sprite
		});
		var maginotShipPartCannon = helper.Content.Ships.RegisterPart("maginotPart.Cannon", new PartConfiguration()
		{
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/cannon_excelsior.png")).Sprite
		});
		var maginotShipPartMissiles = helper.Content.Ships.RegisterPart("maginotPart.Missiles", new PartConfiguration()
		{
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/missiles_excelsior.png")).Sprite
		});
		var maginotShipPartCockpit = helper.Content.Ships.RegisterPart("maginotPart.Cockpit", new PartConfiguration()
		{
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/cockpit_excelsior.png")).Sprite
		});
		var maginotShipPartScaffolding = helper.Content.Ships.RegisterPart("maginotPart.Scaffolding", new PartConfiguration()
		{
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/scaffolding_excelsior.png")).Sprite
		});

		var maginotShipSpriteChassis = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Parts/chassis_excelsior.png")).Sprite;

		/* With the parts and sprites done, we can now create our Ship a bit more easily */
		Maginot_Ship = helper.Content.Ships.RegisterShip("maginotShip", new ShipConfiguration()
		{
			Ship = new StarterShip()
			{
				ship = new Ship()
				{
					/* This is how much hull the ship will start a run with. We recommend matching hullMax */
					hull = 12,
					hullMax = 12,
					shieldMaxBase = 5,
					parts =
					{
						/* This is the order in which the ship parts will be arranged in-game, from left to right. Part1 -> Part2 -> Part3 */
						new Part
						{
							type = PType.wing,
							skin = maginotShipPartWing.UniqueName,
							damageModifier = PDamMod.brittle
						},
						new Part
						{
							type = PType.empty,
							skin = maginotShipPartScaffolding.UniqueName,
						},
						new Part
						{
							type = PType.missiles,
							skin = maginotShipPartMissiles.UniqueName,
						},
						new Part
						{
							type = PType.cockpit,
							skin = maginotShipPartCockpit.UniqueName,
						},
						new Part
						{
							type = PType.cannon,
							skin = maginotShipPartCannon.UniqueName,
							flip = true,
						},
						new Part
						{
							type = PType.empty,
							skin = maginotShipPartScaffolding.UniqueName,
							flip = true,
						},
						new Part
						{
							type = PType.wing,
							skin = maginotShipPartWing.UniqueName,
							flip = true,
							damageModifier = PDamMod.brittle
						}
					}
				},

				/* These are cards and artifacts the ship will start a run with. The recommended card amount is 4, and the recommended artifact amount is 2 to 3 */
				cards =
				{
					new CannonColorless(),
					new CannonColorless(),
					new DodgeColorless(),
					new BasicShieldColorless(),
				},
				artifacts =
				{
					new ShieldPrep(),
					new MaginotArtifactIntermixChamber(),
				}
			},
			ExclusiveArtifactTypes = new HashSet<Type>()
			{
				typeof(MaginotArtifactIntermixChamber),
				typeof(MaginotArtifactReinforcedIntermixChamber),
			},

			UnderChassisSprite = maginotShipSpriteChassis,
			Name = AnyLocalizations.Bind(["ship", "maginotShip", "name"]).Localize,
			Description = AnyLocalizations.Bind(["ship", "maginotShip", "desc"]).Localize
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
			SoloStarters = new StarterDeck
			{
				cards = [
					new MaginotCardBuildUp(),
					new MaginotCardHeavyCaltrops(),
					new MaginotCardMobileFort(),
					new MaginotCardPowerToShields(),
					new CannonColorless(),
					new CannonColorless()
				]
			},
			ExeCardType = typeof(MaginotCardMaginotEXE),
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
				icon = RegisterSprite(package, "assets/Icons/activeShielding.png").Sprite
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

