using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BidirectionalDict
{
	/// <summary>
	/// Represents a bidirectional collection of value pairs.
	/// </summary>
	/// <typeparam name="TFirst">The type of the first values in the dictionary.</typeparam>
	/// <typeparam name="TSecond">The type of the second values in the dictionary.</typeparam>
	public class BiDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>>
											   where TFirst : notnull
											   where TSecond : notnull
	{
		///	<summary>Gets the number of value pairs contained in the <see cref="BiDictionary{TFirst, TSecond}" />.</summary>
		/// <returns>The number of value pairs contained in the <see cref="BiDictionary{TFirst, TSecond}" />.</returns>
		public int Count => _firstToSecond.Count;

		private readonly IDictionary<TFirst, TSecond> _firstToSecond;
		private readonly IDictionary<TSecond, TFirst> _secondToFirst;

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
		public BiDictionary()
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>();
			_secondToFirst = new Dictionary<TSecond, TFirst>();
		}

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that contains elements copied from the specified<see cref= "Dictionary{TFirst, TSecond}" /> and uses the default equality comparer for the key type.</summary>
		/// <param name="dictionary">The <see cref="Dictionary{TFirst, TSecond}" /> whose elements are copied to the new <see cref="BiDictionary{TFirst, TSecond}" />.</param>
		public BiDictionary(Dictionary<TFirst, TSecond> dictionary)
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>(dictionary);
			_secondToFirst = new Dictionary<TSecond, TFirst>(dictionary.ToDictionary(k => k.Value, v => v.Key));
		}

		/// <summary>Initializes a new instance of the <see cref="BiDictionary{TFirst, TSecond}" /> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}" />.</summary>
		/// <param name="firstComparer">The <see cref="IEqualityComparer{TFirst}" /> implementation to use when comparing the first values, or <see langword="null" /> to use the default <see cref="IEqualityComparer{TFirst}" /> for the type of the key.</param>
		/// <param name="secondComparer">The <see cref="IEqualityComparer{TSecond}" /> implementation to use when comparing the first values, or <see langword="null" /> to use the default <see cref="IEqualityComparer{TSecond}" /> for the type of the key.</param>
		public BiDictionary(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
			_secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
		}

		/// <summary>
		/// Tries to add new value pair to the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <param name="second">The second value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool TryAdd(TFirst first, TSecond second)
		{
			if (!_firstToSecond.TryAdd(first, second))
			{
				return false;
			}

			if (!_secondToFirst.TryAdd(second, first))
			{
				_firstToSecond.Remove(first);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Tries to add new value pair to the <see cref="BiDictionary{TFirst, TSecond}"/>. If any of the values already exists, it will be updated.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <param name="second">The second value of the pair</param>
		public void AddOrUpdate(TFirst first, TSecond second)
		{
			if (!_firstToSecond.TryAdd(first, second))
			{
				_firstToSecond[first] = second;
			}

			if (!_secondToFirst.TryAdd(second, first))
			{
				_secondToFirst[second] = first;
			}
		}

		/// <summary>
		/// Tries to remove a value pair from the <see cref="BiDictionary{TFirst, TSecond}"/>, by the first value of the pair.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool TryRemove(TFirst first)
		{
			if (!_firstToSecond.Remove(first, out var second))
			{
				return false;
			}

			_secondToFirst.Remove(second);

			return true;
		}

		/// <summary>
		/// Tries to remove a value pair from the <see cref="BiDictionary{TFirst, TSecond}"/>, by the second value of the pair.
		/// </summary>
		/// <param name="second">The second value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool TryRemove(TSecond second)
		{
			if (!_secondToFirst.Remove(second, out var first))
			{
				return false;
			}

			_firstToSecond.Remove(first);

			return true;
		}

		/// <summary>
		/// Tells if the <see cref="BiDictionary{TFirst, TSecond}"/> contains the first value of the value pair.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool Contains(TFirst first)
			=> _firstToSecond.ContainsKey(first);

		/// <summary>
		/// Tells if the <see cref="BiDictionary{TFirst, TSecond}"/> contains the second value of the value pair.
		/// </summary>
		/// <param name="second">The second value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool Contains(TSecond second)
			=> _secondToFirst.ContainsKey(second);

		/// <summary>
		/// Clears all value pairs in the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		public void Clear()
		{
			_secondToFirst.Clear();
			_firstToSecond.Clear();
		}

		/// <summary>
		/// Gets the element by the specified value of the value pair
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <returns>The second value of the pair</returns>
		public TSecond this[TFirst first]
			=> _firstToSecond[first];

		/// <summary>
		/// Gets the element by the specified value of the value pair
		/// </summary>
		/// <param name="second">The second value of the pair</param>
		/// <returns>The first value of the pair</returns>
		public TFirst this[TSecond second]
			=> _secondToFirst[second];

		/// <summary>
		/// Tries to get a value of the value pair from the <see cref="BiDictionary{TFirst, TSecond}"/>, by the first value of the pair.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <param name="second">The first value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool TryGet(TFirst first, out TSecond second)
			=> _firstToSecond.TryGetValue(first, out second);

		/// <summary>
		/// Tries to get a value of the value pair from the <see cref="BiDictionary{TFirst, TSecond}"/>, by the second value of the pair.
		/// </summary>
		/// <param name="first">The first value of the pair</param>
		/// <param name="second">The second value of the pair</param>
		/// <returns>Returns <see langword="true"/>, if the operation was successful, otherwise returns <see langword="false"/>.</returns>
		public bool TryGet(TSecond second, out TFirst first)
			=> _secondToFirst.TryGetValue(second, out first);

		/// <summary>
		/// Gets the <see cref="IEnumerator{T}"/> of the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		/// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="BiDictionary{TFirst, TSecond}"/></returns>
		public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
			=> _firstToSecond.GetEnumerator();

		/// <summary>
		/// Gets the <see cref="IEnumerator"/> of the <see cref="BiDictionary{TFirst, TSecond}"/>.
		/// </summary>
		/// <returns>The <see cref="IEnumerator"/> of the <see cref="BiDictionary{TFirst, TSecond}"/></returns>
		IEnumerator IEnumerable.GetEnumerator()
			=> _firstToSecond.GetEnumerator();
	}
}
