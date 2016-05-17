using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SelectMapping.Helpers;

namespace SelectMapping.Models
{
    public static class SelectMapper
    {
		private const string SELECT = "$select";
		private const string SUB_PROPS_REGEX = @"\(([A-z]|[0-9]|,|\(.*\))*\)";

		public static Option<IEnumerable<SelectProperty>> GetSelect(this string queryString)
		{
			return queryString
				.SelectString()
				.ToOption()
				.Select(it => it.AsSelectMappings());
		}

		public static string SelectString(this string queryString)
		{
			string selectValue;
			OdataQueries(queryString).TryGetValue(SELECT, out selectValue);
			return selectValue;
		}

		private static IEnumerable<SelectProperty> AsSelectMappings(this string queryString)
		{
			return new Regex(SUB_PROPS_REGEX)
				.Replace(queryString, "")
				.FromCommaSeparatedValues()
				.Select(it => ToSelectMapping(it, queryString));
		}

		private static SelectProperty ToSelectMapping(string propertyName, string queryString)
		{
			var completeProp = new Regex(propertyName + SUB_PROPS_REGEX).Match(queryString).Value;
			var subs = completeProp
				.Without(propertyName)
				.Trim(1)
				.AsSelectMappings();

			return new SelectProperty { Property = propertyName, SubProperties = subs };
		}

		private static Dictionary<string, string> OdataQueries(string queryString)
		{
			var nameValueCollection = HttpUtility.ParseQueryString(queryString);
			return nameValueCollection
				.AllKeys
				.Where(o => o != null && o.StartsWith("$") && nameValueCollection[o] != string.Empty)
				.ToDictionary(t => t, t => nameValueCollection[t]);
		}
	}
}
