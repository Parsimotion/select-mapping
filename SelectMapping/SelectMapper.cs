using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SelectMapping.Helpers;

namespace SelectMapping
{
    public static class SelectMapper
    {
		private const string SELECT = "$select";
		private const string SUB_PROPS_REGEX = @"\(([A-z]|[0-9]|,|\(.*\))*\)";

		public static Option<IEnumerable<SelectMapping>> GetSelect(this string queryString)
		{
			return GetOdataSelect(queryString)
				.Select(it => it.AsSelectMappings());
		}

		private static IEnumerable<SelectMapping> AsSelectMappings(this string queryString)
		{
			return new Regex(SUB_PROPS_REGEX)
				.Replace(queryString, "")
				.FromCommaSeparatedValues()
				.Select(it => ToSelectMapping(it, queryString));
		}

		private static SelectMapping ToSelectMapping(string propertyName, string queryString)
		{
			var completeProp = new Regex(propertyName + SUB_PROPS_REGEX).Match(queryString).Value;
			var subs = completeProp
				.Without(propertyName)
				.Trim(1)
				.AsSelectMappings();

			return new SelectMapping { Property = propertyName, SubProperties = subs };
		}

		private static Option<string> GetOdataSelect(string queryString)
		{
			string selectValue;
			OdataQueries(queryString).TryGetValue(SELECT, out selectValue);
			return selectValue.ToOption();
		}

		private static Dictionary<string, string> OdataQueries(string queryString)
		{
			var nameValueCollection = HttpUtility.ParseQueryString(queryString);
			return nameValueCollection
				.AllKeys
				.Where(o => o != null && o.StartsWith("$"))
				.ToDictionary(t => t, t => nameValueCollection[t]);
		}
	}
}
