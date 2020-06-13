using System.Text;
using TaleWorlds.SaveSystem;

namespace CampaignPacer
{
	/* WHY IS MOST OF THIS OUT-COMMENTED?
	 *
	 * In short, I couldn't get the [damned] SaveSystem to work with my basic value type
	 * generic Dictionary<K, V> types (K: string -> V: long, double, or string). It was
	 * incredibly frustrating at the time trying every permutation of minutiae to get it
	 * it to work, so I reverted to a similar system to previously.
	 */
	class SavedValues
	{
		/* Properties corresponding to saved values.
		 * If the value wasn't set/saved, getters return null.
		 * If null is assigned to the setter, the associated saved value is removed.
		 */
		[SaveableProperty(1)]
		internal long DaysPerSeason { get; set; } = 0;

		[SaveableProperty(2)]
		internal float ScaledPregnancyDuration { get; set; } = 0f;

		//internal long? DaysPerSeason
		//{
		//	get => GetSavedValue(IntMap);
		//	set => SetSavedValue(IntMap, value);
		//}

		//internal double? ScaledPregnancyDuration
		//{
		//	get => GetSavedValue(FloatMap);
		//	set => SetSavedValue(FloatMap, value);
		//}

		internal void Snapshot()
		{
			//Clear();
			DaysPerSeason = Main.Settings.DaysPerSeason;
			ScaledPregnancyDuration = Main.Settings.ScaledPregnancyDuration;
		}

		//internal void SyncData(IDataStore dataStore)
		//{
		//	dataStore.SyncData($"{Main.Name}SavedIntMap", ref _iMap);
		//	dataStore.SyncData($"{Main.Name}SavedFloatMap", ref _fMap);
		//	dataStore.SyncData($"{Main.Name}SavedStringMap", ref _sMap);
		//}

		//public int Count => IntMap.Count + FloatMap.Count + StringMap.Count;

		//public bool IsEmpty => Count == 0;

		//public void Clear()
		//{
		//	IntMap.Clear();
		//	FloatMap.Clear();
		//	StringMap.Clear();
		//}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("{\n");

			//foreach (var item in IntMap)
			//	builder.AppendFormat("{0}{1} = {2}\n", Tab, item.Key, item.Value);

			//foreach (var item in FloatMap)
			//	builder.AppendFormat("{0}{1} = {2}\n", Tab, item.Key, item.Value);

			//foreach (var item in StringMap)
			//	builder.AppendFormat("{0}{1} = {2}\n", Tab, item.Key, item.Value);

			builder.AppendFormat("{0}{1} = {2}\n", Tab, nameof(DaysPerSeason), DaysPerSeason);
			builder.AppendFormat("{0}{1} = {2}\n", Tab, nameof(ScaledPregnancyDuration), ScaledPregnancyDuration);

			builder.Append("}");
			return builder.ToString();
		}

		/* GetSavedValue & SetSavedValue are helpers for saved value property getters/setters, which is why
		 * the keys in both methods use the [CallerMemberName] attribute to prevent the getters/setters
		 * from having to specify their own name and other redundancies regarding the nullable property API.
		 */
		//protected T? GetSavedValue<T>(Dictionary<string, T> map, [CallerMemberName] string key = "") where T : struct
		//{
		//	if (map.TryGetValue(key, out T val))
		//		return val;
		//	else
		//		return null;
		//}

		//protected void SetSavedValue<T>(Dictionary<string, T> map, T? value, [CallerMemberName] string key = "") where T : struct
		//{
		//	if (value != null)
		//		map[key] = value ?? default; // can we use the "damnit operator" (i.e., !) or similar to avoid this null-coalescing?
		//	else
		//		map.Remove(key);
		//}

		/* We actually store our saved settings in three dictionaries-- keyed by value name,
		 * one which holds integral values (64-bit), one which holds floating-point values
		 * (also 64-bit), and one which holds string values.
		 *
		 * Why do this? Save-compatibility paranoia. If new settings are added in the future or
		 * old settings removed, then I want a guarantee that the train will keep running upon
		 * older saves.
		 */
		//protected Dictionary<string, long>   IntMap    { get => _iMap; }
		//protected Dictionary<string, double> FloatMap  { get => _fMap; }
		//protected Dictionary<string, string> StringMap { get => _sMap; }

		//private Dictionary<string, long>   _iMap = new Dictionary<string, long>();
		//private Dictionary<string, double> _fMap = new Dictionary<string, double>();
		//private Dictionary<string, string> _sMap = new Dictionary<string, string>();

		private const string Tab = "    ";
	}
}
