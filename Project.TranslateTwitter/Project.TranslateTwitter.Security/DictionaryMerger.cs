using System.Collections.Generic;
using System.Linq;

namespace Project.TranslateTwitter.Security
{
	public class DictionaryMerger
	{
		/// <summary>
		/// Merge given dictionaries
		/// </summary>
		/// <param name="dictionaries">dictionaries to merge</param>
		/// <param name="overrideDueplicateWithLatterDictionaryValue">
		/// If true and duplicate key exists, then dupe key's values are overriden by latter dictionary values
		/// </param>
		public Dictionary<string, string> MergeDictionaries(
			Dictionary<int, Dictionary<string, string>> dictionaries, 
			bool overrideDueplicateWithLatterDictionaryValue = true)
		{
			Dictionary<string, string> result = new Dictionary<string, string>(dictionaries.Count);
			var query = overrideDueplicateWithLatterDictionaryValue ? dictionaries.Reverse() : dictionaries;

			foreach (var index in query)
			{
				Dictionary<string, string> d2 = index.Value;
				result = result.Concat(d2.Where(x => !result.Keys.Contains(x.Key))).ToDictionary(o => o.Key, o => o.Value);
			}

			return result;
		}
	}
}
