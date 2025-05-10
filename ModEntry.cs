﻿using HarmonyLib;
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
//using TheJazMaster.MoreDifficulties;
//using Cornebre.Maginot.Features;

namespace Cornebre.Maginot;

internal class ModEntry : SimpleMod
{
	internal static ModEntry Instance { get; private set; } = null!;
	internal Harmony Harmony;
	internal IKokoroApi.IV2 KokoroApi;
	internal IDeckEntry MaginotDeck;
	// internal IStatusEntry KnowledgeStatus;
	// internal IStatusEntry LessonStatus;
	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi {get; private set; } = null!;

	/*
	 * The following lists contain references to all types that will be registered to the game.
	 * All cards and artifacts must be registered before they may be used in the game.
	 * In theory only one collection could be used, containing all registrable types, but it is seperated this way for ease of organization.
	 */
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
		typeof(MaginotCardPavis),
		typeof(MaginotCardTwinArtillery),
	];
	private static readonly List<Type> MaginotRareCardTypes = [
		typeof (MaginotCardAllOrBurst),
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
		Harmony = new Harmony("Cornebre.Maginot");
		
		/*
		 * Some mods provide an API, which can be requested from the ModRegistry.
		 * The following is an example of a required dependency - the code would have unexpected errors if Kokoro was not present.
		 * Dependencies can (and should) be defined within the nickel.json file, to ensure proper load mod load order.
		 */
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		/*
		 * A deck only defines how cards should be grouped, for things such as codex sorting and Second Opinions.
		 * A character must be defined with a deck to allow the cards to be obtainable as a character's cards.
		 */
		MaginotDeck = helper.Content.Decks.RegisterDeck("Maginot", new DeckConfiguration
		{
			Definition = new DeckDef
			{
				/*
				 * This color is used in a few places:
				 * TODO On cards, it dictates the sheen on higher rarities, as well as influences the color of the energy cost.
				 * If this deck is given to a playable character, their name will be this color, and their mini will have this color as their border.
				 */
				color = new Color("d5c58a"),

				titleColor = new Color("000000")
			},

			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = RegisterSprite(package, "assets/frame_maginot.png").Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

		/*
		 * All the IRegisterable types placed into the static lists at the start of the class are initialized here.
		 * This snippet invokes all of them, allowing them to register themselves with the package and helper.
		 */
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

		/*
		 * Statuses are used to achieve many mechanics.
		 * However, statuses themselves do not contain any code - they just keep track of how much you have.
		 */
		// KnowledgeStatus = helper.Content.Statuses.RegisterStatus("Knowledge", new StatusConfiguration
		// {
		// 	Definition = new StatusDef
		// 	{
		// 		isGood = true,
		// 		affectedByTimestop = false,
		// 		color = new Color("fbb954"),
		// 		icon = RegisterSprite(package, "assets/knowledge.png").Sprite
		// 	},
		// 	Name = AnyLocalizations.Bind(["status", "knowledge", "name"]).Localize,
		// 	Description = AnyLocalizations.Bind(["status", "knowledge", "desc"]).Localize
		// });
		// LessonStatus = helper.Content.Statuses.RegisterStatus("Lesson", new StatusConfiguration
		// {
		// 	Definition = new StatusDef
		// 	{
		// 		isGood = true,
		// 		affectedByTimestop = false,
		// 		color = new Color("c7dcd0"),
		// 		icon = RegisterSprite(package, "assets/lesson.png").Sprite
		// 	},
		// 	Name = AnyLocalizations.Bind(["status", "lesson", "name"]).Localize,
		// 	Description = AnyLocalizations.Bind(["status", "lesson", "desc"]).Localize
		// });

		/*
		 * Managers are typically made to register themselves when constructed.
		 * _ = makes the compiler not complain about the fact that you are constructing something for seemingly no reason.
		 */
		// _ = new KnowledgeManager(package, helper);
		// _ = new SilentStatusManager();

		/*
		 * Some classes require so little management that a manager may not be worth writing.
		 */
		MaginotActionNastySurprise.Spr = RegisterSprite(package, "assets/Icons/nastySurprise.png").Sprite;
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

