using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	[SaveableClass(1)]
	class SaveBehavior : CampaignBehaviorBase
	{
		[SaveableProperty(1)]
		internal static SimpleTime Time { get; set; }

		public override void RegisterEvents()
		{
			CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, OnBeforeSave);
		}

		public override void SyncData(IDataStore dataStore) { }

		public void OnBeforeSave()
		{
			// synchronize our SimpleTime calendar tracker and the campaign tick tracker before we save

			if (Campaign.Current == null) return;
			
			Time = new SimpleTime(CampaignTime.Now);

			Util.EventTracer.Trace(new List<string>
            {
                $"Campaign tick date:  {CampaignTime.Now}",
                $"Saved calendar date: {Time}",
            });
		}
	}
}
