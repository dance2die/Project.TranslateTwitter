using System.Collections.Generic;
using Project.TranslateTwitter.Security;
using Xunit;

namespace Project.TranslateTwitter.Integration.Test
{
	public class DictionaryTest
	{
		/// <summary>
		/// When two dictionaries with duplicate keys exists, value in the latter dictionary overrides the value.
		/// </summary>
		[Fact]
		public void OverrideDuplicateKeyWithLatterValueWhenMergingDictionaries()
		{
			const string testKey = "key1";
			const string testValue = "value1";

			Dictionary<string, string> sut1 = new Dictionary<string, string>(1) { { testKey, "value1" } };
			Dictionary<string, string> sut2 = new Dictionary<string, string>(1) { { testKey, testValue } };
			Dictionary<string, string> sut3 = new Dictionary<string, string>(1) { { "key2", "value3" } };

			var dictionaryMerger = new DictionaryMerger();
			Dictionary<int, Dictionary<string, string>> mergeInput = new Dictionary<int, Dictionary<string, string>>
			{
				{0, sut1},
				{1, sut2},
				{2, sut3}
			};
			Dictionary<string, string> merged = dictionaryMerger.MergeDictionaries(mergeInput);

			Assert.Equal(2, merged.Count);
			Assert.Equal(testValue, merged[testKey]);
		}
	}
}
