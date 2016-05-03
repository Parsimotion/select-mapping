using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SelectMapping.Helpers
{
	public static class Extensions
	{
		public static bool Contains(this IEnumerable<string> sources, string toCheck, StringComparison comp)
		{
			return sources.Any(s => s.Equals(toCheck, comp));
		}

		/// <summary>
		/// Determines whether a collection is not null and has elements
		/// </summary>
		public static bool HasValue<T>(this IEnumerable<T> enumerable)
		{
			return enumerable != null && enumerable.Any();
		}

		/// <summary>
		/// Finds a property by condition using Single
		/// </summary>
		public static PropertyInfo FindProperty(this Type self, Func<PropertyInfo, bool> condition)
		{
			return self
				.GetProperties()
				.Single(condition);
		}

		/// <summary>
		/// Returns a list of items from a string with values separated by comma 
		/// </summary>
		public static IEnumerable<string> FromCommaSeparatedValues(this string values)
		{
			return values.FromSeparatedValues(',');
		}

		/// <summary>
		/// Returns a list of items from a string with values separated by a separator 
		/// </summary>
		public static IEnumerable<string> FromSeparatedValues(this string values, char separator)
		{
			if (values == "") return new List<string>();

			return values
				.Split(new[] { separator })
				.Select(it => it.Replace(" ", ""))
				.Where(it => it != "");
		}

		public static string Without(this string self, string exclude)
		{
			return self.Replace(exclude, "");
		}

		public static string Trim(this string self, int count)
		{
			var newLength = 2 * count;
			return self.Length < newLength ? "" : self.Substring(count, self.Length - newLength);
		}
	}
}
