using System.Linq;
using FluentAssertions;
using SelectMapping.Helpers;
using SelectMapping.Models;
using SelectMapping.Test.Assertions;
using Xunit;

namespace SelectMapping.Test.Models
{
    public class SelectMapperTest
	{
		const string ID = "id";
		const string LOCATION = "location";
		const string LOCATION_PROPERTIES = "street,city";
		const string CITY_PROPERTIES = "name";
		const string PHONE = "phone";
		const string PHONE_PROPERTIES = "cel";
		const string FILTER = "$filter=PhoneNumber eq 44444444";
		const string PAGINATED = "$skip=1&$top=2&$orderby=name&";
		const string SELECT = ID + "," + LOCATION + "(" + LOCATION_PROPERTIES + "(" + CITY_PROPERTIES + "))," + PHONE + "(" + PHONE_PROPERTIES + ")";
		readonly string QUERYSTRING = $"?$inlinecount=allpages&{PAGINATED + FILTER}&$select={SELECT}&contact=1";

	    [Fact]
	    public void Should_parse_query_string()
	    {
		    QUERYSTRING.ParseSelect().Should().Be(SELECT);
		}

	    [Fact]
	    public void Should_not_parse_empty_select_query_string()
	    {
			"?$select=".ParseSelect().Should().BeNull();
		}

		[Fact]
		public void Should_not_parse_non_select_query_string()
		{
			FILTER.ParseSelect().Should().BeNull();
		}

		[Fact]
		public void Should_return_select_elements()
		{
			QUERYSTRING.GetSelect()
				.Should()
				.HasValue(new[]
				{
					new SelectProperty
					{
						Property = ID
					},
					new SelectProperty
					{
						Property = LOCATION,
						SubProperties =
							new[]
							{
								new SelectProperty { Property = LOCATION_PROPERTIES.FromCommaSeparatedValues().ElementAt(0) },
								new SelectProperty { Property = LOCATION_PROPERTIES.FromCommaSeparatedValues().ElementAt(1), SubProperties = new[] { new SelectProperty { Property = CITY_PROPERTIES } } }
							}
					},
					new SelectProperty
					{
						Property = PHONE,
						SubProperties = PHONE_PROPERTIES.FromCommaSeparatedValues().Select(it => new SelectProperty { Property = it })
					}
				});
		}
	}
}
