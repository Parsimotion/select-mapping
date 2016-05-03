using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SelectMapping.Helpers;

namespace SelectMapping.Models
{
	public class SelectProperty
	{
		public string Property { get; set; }
		public IEnumerable<SelectProperty> SubProperties { get; set; }

		public SelectProperty()
		{
			SubProperties = new List<SelectProperty>();
		}

		public Type ElementType<T>()
		{
			return ElementType(typeof (T));
		}
		
		public Type ElementType(Type type)
		{
			var propertyType = type
				.FindProperty(IsProperty)
				.PropertyType;

			return propertyType.GetElementType() ?? propertyType; // For arrays
		}

		public IEnumerable<string> IgnoredProperties<T>()
		{
			return IgnoredProperties(typeof (T));
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
