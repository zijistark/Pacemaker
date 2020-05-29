using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace QuickTime
{
	public class SubModule : MBSubModuleBase
	{
		public static readonly string ID = typeof(SubModule).Namespace;
		public static readonly string Name = "Swift Time";
		public static readonly string Version = "v0.1.0";
		public static readonly string HarmonyID = "com.zijistark.bannerlord." + ID.ToLower();

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			var harmony = new Harmony(HarmonyID);
			harmony.PatchAll();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!_loaded)
			{
				InformationManager.DisplayMessage(new InformationMessage($"~ Loaded {Name} {Version}", Color.FromUint(0x00007CD7)));
				_loaded = true;
			}
		}

		private bool _loaded = false;
	}
}