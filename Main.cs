using System.Collections.Generic;
using System.ComponentModel;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CampaignPacer
{
	public class Main : MBSubModuleBase
	{
		/* Semantic Versioning (https://semver.org): */
		public const int SemVerMajor = 0;
		public const int SemVerMinor = 8;
		public const int SemVerPatch = 0;
		public const string SemVerSpecial = "alpha0";
		public static readonly string Version = $"{SemVerMajor}.{SemVerMinor}.{SemVerPatch}{((SemVerSpecial != null) ? $"-{SemVerSpecial}" : "")}";

		public static readonly string Name = typeof(Main).Namespace;
		public const string DisplayName = "Campaign Pacer"; // to be shown to humans in-game
		public static readonly string HarmonyDomain = "com.zijistark.bannerlord." + Name.ToLower();

		internal static Settings Config = null;
		internal static TimeParams TimeParam;
		internal static Harmony Harmony = null;

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Util.EnableLog = true; // enable various debug logging
			Util.EnableTracer = true; // enable code event tracing (requires enabled logging to actually go anywhere)
			Harmony = new Harmony(HarmonyDomain);
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			var trace = new List<string>();

			if (!_loaded)
			{
				trace.Add("Module is loading for the first time...");

				if (Settings.Instance == null)
				{
					Config = new Settings(); // use defaults
					trace.Add("Settings.Instance was NULL! Using default configuration instead.");
				}
				else
					Config = Settings.Instance;

				// register for settings property-changed events
				Config.PropertyChanged += OnSettingsPropertyChanged;

				trace.AddRange(new List<string>
				{
					string.Empty,
					"Settings:",
				});

				trace.AddRange(Config.ToStringLines(indentSize: 2));

				SetTimeParams(trace);
				Harmony.PatchAll();

				InformationManager.DisplayMessage(new InformationMessage($"Loaded {DisplayName} v{Version}", Color.FromUint(0x00F16D26)));
				_loaded = true;
			}
			else
				trace.Add("Module was already fully loaded.");

			if (Util.EnableTracer)
				Util.EventTracer.Trace(trace);
			else
				Util.Log.ToFile(trace);
		}

		protected override void OnGameStart(Game game, IGameStarter starterObject)
		{
			base.OnGameStart(game, starterObject);
			var trace = new List<string> { $"Game type: [{game.GameType.GetType().Name}] {game.GameType}" };

			if (game.GameType is Campaign)
			{
				CampaignGameStarter initializer = (CampaignGameStarter)starterObject;
				AddBehaviors(initializer, trace);
			}

			Util.EventTracer.Trace(trace);
		}

		protected void AddBehaviors(CampaignGameStarter gameInitializer, List<string> trace)
		{
			gameInitializer.AddBehavior(new SaveBehavior());
			trace.Add($"Behavior added: {typeof(SaveBehavior).FullName}");

			if (Util.EnableTracer && Util.EnableLog)
			{
				gameInitializer.AddBehavior(new TickTraceBehavior());
				trace.Add($"Behavior added: {typeof(TickTraceBehavior).FullName}");
			}
		}

		protected static void SetTimeParams(List<string> trace)
		{
			trace.Add("Setting new time parameters...");
			TimeParam = new TimeParams(Config.DaysPerSeason, trace);
			trace.Add(string.Empty);
			trace.AddRange(TimeParam.ToStringLines(indentSize: 2));
		}

		protected static void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (sender is Settings && args.PropertyName == Settings.SaveTriggered)
			{
				var trace = new List<string> { "Received save-triggered property changed event from Settings..." };
				SetTimeParams(trace);
				Util.EventTracer.Trace(trace);
			}
		}

		private bool _loaded = false;
	}
}
