using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syw.Client.Common;

namespace Syw.Client
{
	internal static class ObjectParametersExtensions
	{
		public static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> ParametersModelCache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

		public static IDictionary<string, object> GetParameters(this object parametersModel)
		{
			if (parametersModel == null)
				return new Dictionary<string, object>();

			var type = parametersModel.GetType();

			var model = parametersModel as Dictionary<string, object>;
			if (model != null)
				return model;

			if (!ParametersModelCache.ContainsKey(type))
				ParametersModelCache.TryAdd(type, type.GetProperties(BindingFlags.Instance | BindingFlags.Public));

			var modelPropertiesInfo = ParametersModelCache[type];

			return modelPropertiesInfo
				.DistinctBy(x => x.Name).ToDictionary(x => x.Name, x => x.GetValue(parametersModel, null));
		}

		private static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> target, params Func<T, object>[] keySelectors)
		{
			return target.Distinct(For(keySelectors));
		}

		private static IEqualityComparer<T> For<T>(params Func<T, object>[] keySelectors)
		{
			return new CompositeKeyEqualityComparer<T>(keySelectors);
		}
	}
}