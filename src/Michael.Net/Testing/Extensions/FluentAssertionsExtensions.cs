using FluentAssertions.Equivalency;
using System.Linq.Expressions;

namespace Michael.Net.Testing.Extensions
{
    public static class FluentAssertionsExtensions
    {
        public static EquivalencyAssertionOptions<TExpectation> Excluding<TExpectation>(
            this EquivalencyAssertionOptions<TExpectation> options, params Expression<Func<TExpectation, object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                options.Excluding(expression);
            }

            return options;
        }

        public static EquivalencyAssertionOptions<TSubject> WithPropertyAssertion<TSubject, TProperty>(
            this EquivalencyAssertionOptions<TSubject> options,
            Expression<Func<TSubject, TProperty>> propertySelector,
            Action<IAssertionContext<TProperty>> propertyAssertion)
        {
            var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;

            return options
                .Using(propertyAssertion)
                .When(info => info.Path.EndsWith(propertyName));
        }
    }
}
