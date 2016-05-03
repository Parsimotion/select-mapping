using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SelectMapping.Helpers
{
	public struct Option<T>
	{
		private readonly T value;
		private readonly bool hasValue;
		/// <summary>
		/// Indicates whether the option holds a value or not
		/// </summary>
		public bool HasValue
		{
			get { return hasValue; }
		}

		private Option(T value)
		{
			this.value = value;
			hasValue = true;
		}

		/// <summary>
		/// Unwraps the option.
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns>Specified, default value if the option is empty, or the option's value if present</returns>
		/// <remarks>It's recommended to use the <see cref="GetOrElse(Func{TResult})"/> when passing a parameter expression to be evaluated.
		/// In this case, the condition will get evaluated AFTER evaluation of the parameter, which may be costly.</remarks>
		public T GetOrElse(T defaultValue)
		{
			return HasValue ? value : defaultValue;
		}

		/// <summary>
		/// Unwraps the option
		/// </summary>
		/// <param name="defaultValue">Function returning the default value</param>
		/// <returns>The evaluation of the specified function if option is empty, or the option's value if present</returns>
		/// <remarks>Recommended overload. The function returning defaultValue will only get evaluated if
		/// the option is empty.</remarks>
		public T GetOrElse(Func<T> defaultValue)
		{
			return HasValue ? value : defaultValue();
		}

		/// <summary>
		/// Unwraps the option
		/// </summary>
		/// <param name="defaultValue">Function returning the default value</param>
		/// <returns>The evaluation of the specified function if option is empty, or the option's value if present</returns>
		/// <remarks>Recommended overload. The function returning defaultValue will only get evaluated if
		/// the option is empty.</remarks>
		public async Task<T> GetOrElse(Func<Task<T>> defaultValue)
		{
			return HasValue ? value : await defaultValue();
		}

		/// <summary>
		/// Unwraps the option
		/// </summary>
		/// <returns>The default value for the type if option is empty, or the option's value if present</returns>
		public T GetOrDefault()
		{
			return GetOrElse(default(T));
		}

		/// <summary>
		/// Return the option's value if present or throws the given exception
		/// </summary>
		public T GetOrThrow(Exception e)
		{
			Func<T> defaultValue = () => { throw e; };
			return GetOrElse(defaultValue);
		}

		public override string ToString()
		{
			return !HasValue ? "None" : value.ToString();
		}

		public static readonly Option<T> None = new Option<T>();

		public static Option<T> Some(T value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			return new Option<T>(value);
		}

		public static Option<T> From(T value)
		{
			if (value == null)
				return None;

			return Some(value);
		}

		/// <summary>
		/// Applies the specified function to option's value (if the option is non-empty) and yields a new option.
		/// </summary>
		/// <typeparam name="U"></typeparam>
		/// <param name="func"></param>
		/// <returns>A new option after applying the function to current option's value if the current option is non-empty, <see cref="Option{T}.None"/> otherwise.</returns>
		public Option<U> Select<U>(Func<T, U> func)
		{
			if (!HasValue)
				return Option<U>.None;

			return Option<U>.From(func(value));
		}

		/// <summary>
		/// Applies a specified function to the option's value and yields a new option if the option is non-empty.
		/// <remarks>Different from <see cref="Select{U}"/>, expects a function that returns an <see cref="Option{T}"/></remarks>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="func"></param>
		/// <returns>A new option if the current option is non-empty, <see cref="Option{T}.None"/> otherwise.</returns>
		public Option<U> SelectMany<U>(Func<T, Option<U>> func)
		{
			if (!HasValue)
				return Option<U>.None;

			return func(value);
		}

		/// <summary>
		/// Filters the option by the passed predicate function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <returns>Returns the option if the option is non-empty and the value underneath satisfied the predicate, <see cref="Option{T}.None"/>  otherwise.</returns>
		public Option<T> Where(Func<T, bool> func)
		{
			if (HasValue && func(value))
				return this;

			return None;
		}

		/// <summary>
		/// Continues with the option if the bool is true
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="aBool"></param>
		/// <returns>Returns the option if the option is non-empty and the bool is true, <see cref="Option{T}.None"/>  otherwise.</returns>
		public Option<T> IfTrue(bool aBool)
		{
			if (HasValue && aBool)
				return this;

			return None;
		}

		/// <summary>
		/// Checks if the value 'exists' inside the option.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <returns><code>true</code> if the option is not empty and if it satisfied the predicate, <see cref="Option{T}.None"/> otherwise.</returns>
		public bool Any(Func<T, bool> func)
		{
			return HasValue && func(value);
		}

		/// <summary>
		/// Executes a specified action on the option, if the option is non-empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		public void ForEach(Action<T> action)
		{
			if (HasValue)
				action(value);
		}

		/// <summary>
		/// Returns the <paramref name="alternative"/> if the current option is empty. Returns the option itself otherwise.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="alternative"></param>
		/// <remarks>It's recommended to use <see cref="OrElse{T}(Func{Option{T}}"/> overload.</remarks>
		/// <returns></returns>
		public Option<T> OrElse(Option<T> alternative)
		{
			return !HasValue ? alternative : this;
		}

		/// <summary>
		/// Checks whether the current option is empty; if it is, the <paramref name="alternative"/> function is evaluated and the result is returned. Otherwise,
		/// the calling instance is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="alternative"></param>
		/// <returns>The current option if it's non-empty and the evaluation result of the alternative function otherwise.</returns>
		public Option<T> OrElse(Func<Option<T>> alternative)
		{
			return !HasValue ? alternative() : this;
		}

		/// <summary>
		/// Converts the option to a sequence (<see cref="IEnumerable{T}"/>)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>One element sequence containing the option's value if the option was non-empty, empty sequence otherwise</returns>
		public IEnumerable<T> ToEnumerable()
		{
			if (!HasValue) yield break;

			yield return value;
		}

		public static implicit operator Option<T>(T value)
		{
			return From(value);
		}

		public void Match(Action<T> action, Action @else = null)
		{
			if (HasValue)
				action(value);
			else
				if (@else != null) @else();
		}

		public async Task AsyncMatch(Func<T, Task> action)
		{
			if (this.HasValue)
				await action(this.value);
		}
	}

	public static class OptionExtensions
	{
		/// <summary>
		/// Wraps the specified object in an option. If the object is null, returns <see cref="Option{T}.None"/>, otherwise creates a new option
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns>A new <see cref="Option{T}"/></returns>
		public static Option<T> ToOption<T>(this T value)
		{
			return Option<T>.From(value);
		}

		/// <summary>
		/// Wraps the specified collection in an option. If the collection is empty, returns <see cref="Option{T}.None"/>, otherwise creates a new option
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns>A new <see cref="Option{T}"/></returns>
		public static Option<IEnumerable<T>> AsOption<T>(this IEnumerable<T> value)
		{
			return value.Any() ? value.ToOption() : Option<IEnumerable<T>>.None;
		}


		/// <summary>
		/// Returns the only element of a sequence that satisfies a specified condition or <see cref="Option{TElement}.None"/> if no such element exists
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		public static Option<TElement> Detect<TElement>(this IQueryable<TElement> self, Expression<Func<TElement, bool>> predicate)
		{
			return self.SingleOrDefault(predicate).ToOption();
		}

		/// <summary>
		/// Returns the only element of a sequence that satisfies a specified condition or <see cref="Option{TElement}.None"/> if no such element exists
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		public static Option<TElement> Detect<TElement>(this IEnumerable<TElement> self, Func<TElement, bool> predicate)
		{
			return self.SingleOrDefault(predicate).ToOption();
		}

		/// <summary>
		/// Returns the first element of a sequence or <see cref="Option{TElement}.None"/> if no such element exists
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		public static Option<TElement> FirstOrNone<TElement>(this IEnumerable<TElement> self)
		{
			return self.FirstOrDefault().ToOption();
		}

		/// <summary>
		/// Returns the first element of a sequence that satisfies a specified condition or <see cref="Option{TElement}.None"/> if no such element exists
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		public static Option<TElement> FirstOrNone<TElement>(this IEnumerable<TElement> self, Func<TElement, bool> predicate)
		{
			return self.FirstOrDefault(predicate).ToOption();
		}

		/// <summary>
		/// Returns a collection containing all the non empty values 
		/// </summary>
		public static IEnumerable<TElement> FilterNonEmpty<TElement>(this IEnumerable<Option<TElement>> self)
		{
			return self.Where(it => it.HasValue).Select(it => it.GetOrDefault());
		}

		/// <summary>
		/// Try cast an object to T. Returns that <see cref="Option{T}"/>
		/// </summary>
		public static Option<T> Cast<T>(this object self) where T : class
		{
			return (self as T).ToOption();
		}

		public static Option<TValue> Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, TKey key)
		{
			TValue result;

			return self.TryGetValue(key, out result)
				? Option<TValue>.Some(result)
				: Option<TValue>.None;
		}
	}
}
