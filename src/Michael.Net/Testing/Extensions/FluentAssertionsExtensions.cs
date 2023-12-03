using FluentAssertions.Equivalency;
using System.Linq.Expressions;

namespace Michael.Net.Testing.Extensions
{
    public static class FluentAssertionsExtensions
    {
        public static EquivalencyAssertionOptions<TExpectation> Excluding<TExpectation>(
            this EquivalencyAssertionOptions<TExpectation> options, params Expression<Func<TExpectation, object>>[] properties)
        {
            foreach (var property in properties)
            {
                options.Excluding(property);
            }

            return options;
        }

        public static EquivalencyAssertionOptions<TExpectation> ExcludingCollectionProperty<TExpectation>(
            this EquivalencyAssertionOptions<TExpectation> options, params Expression<Func<TExpectation, object>>[] properties)
        {
            static string GetPropertyName<T>(Expression<Func<T, object>> expression)
            {
                if (expression.Body is MemberExpression memberExpression)
                {
                    return memberExpression.Member.Name;
                }

                throw new ArgumentException("Invalid expression", nameof(expression));
            }

            foreach (var property in properties)
            {
                var name = GetPropertyName(property);
                options.Excluding((IMemberInfo m) => m.Path.EndsWith(name));
            }

            return options;
        }

        public static EquivalencyAssertionOptions<TExpectation> WithPropertyAssertion<TExpectation, TProperty>(
            this EquivalencyAssertionOptions<TExpectation> options,
            Expression<Func<TExpectation, TProperty>> propertySelector,
            Action<IAssertionContext<TProperty>> propertyAssertion)
        {
            var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;

            return options
                .Using(propertyAssertion)
                .When(info => info.Path.EndsWith(propertyName));
        }
    }
}
