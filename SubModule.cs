using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CampaignPacer
{
	public class SubModule : MBSubModuleBase
	{
		/* Semantic Versioning (https://semver.org): */
		public const int SemVerMajor = 0;
		public const int SemVerMinor = 3;
		public const int SemVerPatch = 0;
		public const string SemVerSpecial = null; // valid would be "alpha2" or "beta7" or "rc1", e.g.
		public static readonly string Version = $"{SemVerMajor}.{SemVerMinor}.{SemVerPatch}{((SemVerSpecial != null) ? $"-{SemVerSpecial}" : "")}";
		
		public static readonly string Name = typeof(SubModule).Namespace; // used for both Id and human-readable Name of this module
		public static readonly string HarmonyDomain = "com.zijistark.bannerlord." + Name.ToLower();

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			var harmony = new Harmony(HarmonyDomain);
			harmony.PatchAll();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!_loaded)
			{
				InformationManager.DisplayMessage(new InformationMessage($"Loaded {Name} v{Version}!", Color.FromUint(0x00F16D26)));
				_loaded = true;
			}
		}

		private bool _loaded = false;
	}
}