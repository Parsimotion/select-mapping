﻿using FluentAssertions;
using SelectMapping.Models;
using Xunit;

namespace SelectMapping.Test.Models
{
	public class SelectPropertyTest
	{
		private const string LOCATION = "Location";
		private const string ADDRESS = "Address";
		private const string CITY = "City";

		[Fact]
		public void Should_return_matching_name_property_type()
		{
			SelectLocation.ElementType<Person>().ShouldBeEquivalentTo(typeof (Location));
		}

		[Fact]
		public void Should_return_matching_name_element_type_for_arrays()
		{
			SelectLocation.ElementType<Group>().ShouldBeEquivalentTo(typeof (Location));
		}

		[Fact]
		public void Should_return_not_mapped_property_names()
		{
			SelectLocation.IgnoredProperties<Location>().ShouldBeEquivalentTo(new[] {ADDRESS});
		}

		[Fact]
		public void Should_not_return_mapped_property_names_when_it_hasnt_subproperties()
		{
			PoorSelectProperty.IgnoredProperties<Location>().Should().BeEmpty();
		}

		[Fact]
		public void Should_ignore_property_name_case()
		{
			PoorSelectProperty.ElementType<Person>().ShouldBeEquivalentTo(typeof (Location));
		}

		[Fact]
		public void Should_be_selected_its_property()
		{
			SelectLocation.IsSelected(LOCATION).Should().BeTrue();
		}

		[Fact]
		public void Should_be_selected_any_subproperty()
		{
			SelectLocation.IsSelected(CITY).Should().BeTrue();
		}

		[Fact]
		public void Should_not_be_selected_unknown_property()
		{
			SelectLocation.IsSelected(ADDRESS).Should().BeFalse();
		}


		private static SelectProperty SelectLocation
		{
			get
			{
				return new SelectProperty {Property = LOCATION, SubProperties = new[] {new SelectProperty {Property = CITY}}};
			}
		}

		private static SelectProperty PoorSelectProperty
		{
			get { return new SelectProperty { Property = "lOcAtIoN" }; }
		}
	}

	internal class Person
	{
		public Location Location { get; set; }
	}

	internal class Group
	{
		public Location[] Location { get; set; }
	}

	internal class Location
	{
		public string City { get; set; }
		public string Address { get; set; }
	}
}
