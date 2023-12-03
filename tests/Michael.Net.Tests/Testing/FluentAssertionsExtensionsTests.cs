using FluentAssertions;
using Michael.Net.Testing.Extensions;
using Xunit;
using Xunit.Sdk;

namespace Michael.Net.Tests.Testing
{
    public class FluentAssertionsExtensionsTests
    {
        [Fact]
        public void WhenComparingWithPropertyAssertion_AssertionIsUsedAndWorks()
        {
            //Given
            var class1 = new Class1() { Id = 1, Name = "ThisName" };
            var class2 = new Class1() { Id = 1, Name = "ThisName " };

            //When
            var assert = () => class1.Should().BeEquivalentTo(class2, 
                o => o.WithPropertyAssertion(p => p.Name, c => c.Subject.Should().Be(c.Expectation.Trim())));

            //Then
            assert.Should().NotThrow();
        }

        [Fact]
        public void WhenComparingWithPropertyAssertion_AssertionIsUsedAndFails()
        {
            //Given
            var class1 = new Class1() { Id = 1, Name = "ThisName " };
            var class2 = new Class1() { Id = 1, Name = "ThisName " };

            //When
            var assert = () => class1.Should().BeEquivalentTo(class2,
                o => o.WithPropertyAssertion(p => p.Name, c => c.Subject.Should().Be(c.Expectation.Trim())));

            //Then
            assert.Should().Throw<XunitException>()
                .WithMessage("*Expected property class1.Name to be \"ThisName\", but it has unexpected whitespace at the end.*");
        }

        class Class1
        {
            public int Id { get; set; }
            public string Name { get; set; } = default!;
        }

        [Fact]
        public void WhenNoExcluding_Fails()
        {
            //Given
            var class1 = new Class2() { Id = 1, Classes1 = new Class1[] { new() { Id = 1, Name = "This name" } } };
            var class2 = new Class2() { Id = 1, Classes1 = new Class1[] { new() { Id = 1, Name = "" } } };

            //When
            var assert = () => class1.Should().BeEquivalentTo(class2);

            //Then
            assert.Should().Throw<XunitException>()
                .WithMessage(@"*Expected property class1.Classes1[0].Name to be """" with a length of 0, but ""This name"" has a length of 9*");
        }

        [Fact]
        public void WhenExcludingCollectionProperty_Works()
        {
            //Given
            var class1 = new Class2() { Id = 1, Classes1 = new Class1[] { new() { Id = 1, Name = "This name" } } };
            var class2 = new Class2() { Id = 1, Classes1 = new Class1[] { new() { Id = 1, Name = "" } } };

            //When
            var assert = () => class1.Should().BeEquivalentTo(class2, o => o.ExcludingCollectionProperty(c => c.Classes1.First().Name));

            //Then
            assert.Should().NotThrow();
        }

        class Class2
        {
            public int Id { get; set; }
            public IEnumerable<Class1> Classes1 { get; set; } = default!;
        }
    }
}
