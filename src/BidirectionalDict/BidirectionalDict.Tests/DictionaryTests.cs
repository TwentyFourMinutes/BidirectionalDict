using BidirectionalDict.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BidirectionalDict.Tests
{
	public class BiDitionaryTests : BaseDictionaryTests
	{
		public BiDitionaryTests() : base(new BiDictionary<FooEnum, string>(Dictionary))
		{
		}
	}

	public class ConcurrentBiDitionaryTests : BaseDictionaryTests
	{
		public ConcurrentBiDitionaryTests() : base(new ConcurrentBiDictionary<FooEnum, string>(Dictionary))
		{
		}
	}

	public class MultiThreadedConcurrentBiDitionaryTests
	{
		public ConcurrentBiDictionary<int, string> Dictionary;

		public MultiThreadedConcurrentBiDitionaryTests()
		{
			Dictionary = new ConcurrentBiDictionary<int, string>();
		}

		[Fact]
		public void AddingValues()
		{
			var rnd = new Random();

			var pResult = Parallel.For(0, 5000, i =>
			{
				Dictionary.TryAdd(rnd.Next(0, 100000), rnd.Next(0, 1000).ToString());
			});

			Parallel.For(0, 1000, i =>
			{
				Dictionary.TryRemove(rnd.Next(0, 100000));
				Dictionary.TryRemove(rnd.Next(0, 100000).ToString());
			});

			Assert.True(pResult.IsCompleted);

			AreDictionaryPairsSynced();
		}

		private void AreDictionaryPairsSynced()
		{
			foreach (var kvp in Dictionary)
			{
				if (Dictionary.TryGet(kvp.Value, out var result))
				{
					Assert.Equal(kvp.Key, result);
				}
				else
				{
					Assert.True(false);
				}
			}

			Assert.True(Dictionary.IsSynced);
		}
	}

	public class ReadOnlyBiDitionaryTests : BaseReadOnlyDictionaryTests
	{
		public ReadOnlyBiDitionaryTests() : base(new ReadOnlyBiDictionary<FooEnum, string>(Dictionary))
		{
		}
	}
}
