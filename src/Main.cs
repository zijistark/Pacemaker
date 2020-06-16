using System.Collections.Generic;
using System.ComponentModel;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Pacemaker
{
	public class Main : MBSubModuleBase
	{
		/* Semantic Versioning (https://semver.org): */
		public const int SemVerMajor = 0;
		public const int SemVerMinor = 9;
		public const int SemVerPatch = 1;
		public const string SemVerSpecial = null;
		public static readonly string Version = SemVerMajor.ToString() + '.' +
			SemVerMinor.ToString() + '.' +
			SemVerPatch.ToString() + (
				(SemVerSpecial != null)
				? '-' + SemVerSpecial
				: string.Empty
			);

		public static readonly string Name = typeof(Main).Namespace;
		public static readonly string DisplayName = Name; // to be shown to humans in-game
		public static readonly string HarmonyDomain = "com.zijistark.bannerlord." + Name.ToLower();

		internal static Settings Settings = null;
		internal static TimeParams TimeParam;
		internal static Harmony Harmony = null;

		protected bool EnableTickTracer = true;

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

			if (_loaded)
				trace.Add("Module was already loaded.");
			else
				trace.Add("Module is loading for the first time...");

			if (Settings.Instance != null && Settings.Instance != Settings)
			{
				Settings = Settings.Instance;

				// register for settings property-changed events
				Settings.PropertyChanged += Settings_OnPropertyChanged;

				trace.Add(string.Empty);
				trace.Add("Loaded Settings:");
				trace.AddRange(Settings.ToStringLines(indentSize: 4));
				trace.Add(string.Empty);

				SetTimeParams(new TimeParams(Settings.DaysPerSeason), trace);
			}

			if (!_loaded)
			{
				Harmony.PatchAll();

				InformationManager.DisplayMessage(new InformationMessage($"Loaded {DisplayName} v{Version}", Color.FromUint(0x00F16D26)));
				_loaded = true;
			}

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

			if (EnableTickTracer && Util.EnableTracer && Util.EnableLog)
			{
				gameInitializer.AddBehavior(new TickTraceBehavior());
				trace.Add($"Behavior added: {typeof(TickTraceBehavior).FullName}");
			}
		}

		internal static TimeParams SetTimeParams(TimeParams newParams, List<string> trace)
		{
			trace.Add("Setting time parameters...");

			var oldParams = TimeParam;
			TimeParam = newParams;

			trace.Add(string.Empty);
			trace.AddRange(TimeParam.ToStringLines(indentSize: 4));

			return oldParams;
		}

		protected static void Settings_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (sender is Settings && args.PropertyName == Settings.SaveTriggered)
			{
				var trace = new List<string> { "Received save-triggered event from Settings..." };
				trace.Add(string.Empty);
				trace.Add("New Settings:");
				trace.AddRange(Settings.ToStringLines(indentSize: 4));
				trace.Add(string.Empty);
				SetTimeParams(new TimeParams(Settings.DaysPerSeason), trace);
				Util.EventTracer.Trace(trace);
			}
		}

		private bool _loaded = false;
	}
}
