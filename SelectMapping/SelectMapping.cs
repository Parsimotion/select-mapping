using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SelectMapping.Helpers;

namespace SelectMapping
{
	public class SelectMapping
	{
		public string Property { get; set; }
		public IEnumerable<SelectMapping> SubProperties { get; set; }

		public SelectMapping()
		{
			SubProperties = new List<SelectMapping>();
		}

		public Type ElementType(Type type)
		{
			var propertyType = type
				.FindProperty(IsProperty)
				.PropertyType;

			return propertyType.GetElementType() ?? propertyType; // For arrays
		}

		public IEnumerable<string> IgnoredProperties(Type elementType)
		{
			return elementType
				.GetProperties()
				.Where(ShouldIgnore)
				.Select(it => it.Name);
		}

		private bool ShouldIgnore(PropertyInfo property)
		{
			return SubProperties.HasValue() && !SubProperties.Select(it => it.Property).Contains(property.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool IsProperty(PropertyInfo property)
		{
			return Property.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
