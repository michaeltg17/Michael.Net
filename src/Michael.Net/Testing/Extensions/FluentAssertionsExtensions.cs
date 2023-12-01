using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
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

        //public static EquivalencyAssertionOptions<TExpectation> WithPropertyConstraint<TExpectation, TProperty>(
        //    this EquivalencyAssertionOptions<TExpectation> options, Action<IAssertionContext<TProperty>> action, Expression<TProperty> propertySelector)
        //{
        //    options
        //        .Using<TProperty>(action)
        //        .When(info => info.Path.EndsWith(propertySelector.Name));
        //}

        public static EquivalencyAssertionOptions<TSubject> WithPropertyAssertion<TSubject, TProperty>(
            this EquivalencyAssertionOptions<TSubject> options,
            Action<IAssertionContext<TProperty>> action,
            Expression<Func<TSubject, TProperty>> propertySelector)
        {
            var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;

            return options
                .Using<TProperty>(action)
                .When(info => info.Path.EndsWith(propertyName));
        }
    }
}
