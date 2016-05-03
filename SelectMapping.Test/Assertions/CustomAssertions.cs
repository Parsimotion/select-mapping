using FluentAssertions.Primitives;

namespace SelectMapping.Test.Assertions
{
	public abstract class CustomAssertions<T> : ReferenceTypeAssertions<T, CustomAssertions<T>>
	{
		protected readonly T subject;

		protected CustomAssertions(T subject)
		{
			this.subject = subject;
		}

		protected override string Context
		{
			get { return "Custom"; }
		}
	}
}