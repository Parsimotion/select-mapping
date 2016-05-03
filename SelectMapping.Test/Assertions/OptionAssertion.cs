using FluentAssertions;
using SelectMapping.Helpers;

namespace SelectMapping.Test.Assertions
{
	public static class OptionAssertionsUtils
	{
		public static OptionAssertion<T> Should<T>(this Option<T> self)
		{
			return new OptionAssertion<T>(self);
		}
	}

	public class OptionAssertion<T> : CustomAssertions<Option<T>>
	{
		public OptionAssertion(Option<T> subject) : base(subject) { }

		public void HasValue(T expectedValue)
		{
			this.subject.GetOrDefault().ShouldBeEquivalentTo(expectedValue, opt => opt.IncludingAllRuntimeProperties());
		}

		public void BeNone()
		{
			this.subject.ShouldBeEquivalentTo(Option<T>.None);
		}
	}
}