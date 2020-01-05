using BidirectionalDict.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BidirectionalDict.Tests
{


	public abstract class BaseDictionaryTests : BaseReadOnlyDictionaryTests
	{
		private readonly IBiDictionary<FooEnum, string> _dictionary;

		public BaseDictionaryTests(IBiDictionary<FooEnum, string> dictionary) : base(dictionary)
		{
			_dictionary = dictionary;
		}

		[Fact]
		public void BlockAlreadyExistingValues()
		{
			var result = _dictionary.TryAdd(FooEnum.Foo, "Fooo");
			Assert.False(result);

			result = _dictionary.TryAdd(FooEnum.Foo, "Barr");
			Assert.False(result);

			result = _dictionary.TryAdd(FooEnum.Bar, "Fooo");
			Assert.False(result);

		}

		[Fact]
		public void RemoveValue()
		{
			_dictionary.TryRemove(FooEnum.Foo);

			Assert.False(_dictionary.Contains("Fooo"));
			Assert.False(_dictionary.Contains(FooEnum.Foo));
		}

		[Fact]
		public void ClearAllValues()
		{
			_dictionary.Clear();

			Assert.Equal(0, _dictionary.Count);
		}
	}

	public abstract class BaseReadOnlyDictionaryTests
	{
		public static readonly Dictionary<FooEnum, string> Dictionary = new Dictionary<FooEnum, string>
		{
			{ FooEnum.Bar, "Barr"},
			{ FooEnum.Foo, "Fooo"},
			{ FooEnum.FooBar, "FoooBarr"}
		};

		private readonly IReadOnlyBiDictionary<FooEnum, string> _dictionary;

		public BaseReadOnlyDictionaryTests(IReadOnlyBiDictionary<FooEnum, string> dictionary)
		{
			_dictionary = dictionary;
		}

		[Fact]
		public void RetriveCorrectValueForEachKey()
		{
			_dictionary.TryGet(FooEnum.Bar, out var second);
			Assert.Equal("Barr", second);

			_dictionary.TryGet("Barr", out var first);
			Assert.Equal(FooEnum.Bar, first);
		}

		[Fact]
		public void CanForeachOver()
		{
			var index = 0;

			foreach (var item in _dictionary)
			{
				index++;
			}

			Assert.Equal(index, _dictionary.Count);
		}
	}
}
