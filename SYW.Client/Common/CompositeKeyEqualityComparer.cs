using System;
using System.Collections.Generic;
using System.Linq;

namespace Syw.Client.Common
{
	internal class CompositeKeyEqualityComparer<T> : IEqualityComparer<T>
	{
		private readonly IList<Func<T, object>> _keySelectors;

		public CompositeKeyEqualityComparer(IList<Func<T, object>> keySelectors)
		{
			_keySelectors = keySelectors;
		}

		public bool Equals(T x, T y)
		{
			if (Object.ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;

			return Enumerable.All(_keySelectors, ks => Object.Equals(ks(x), ks(y)));
		}

		public int GetHashCode(T obj)
		{
			return Enumerable.Aggregate(_keySelectors, 0, (h, ks) => (h * 397) ^ ks(obj).GetHashCode());
		}
	}
}