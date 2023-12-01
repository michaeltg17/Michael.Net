using FluentAssertions;
using Michael.Net.Testing.Extensions;
using Xunit;

namespace Michael.Net.Tests.Testing
{
    public class FluentAssertionsExtensionsTests
    {
        [Fact]
        public void Test()
        {
            //Given
            var class1 = new Class1() { Id = 1, Name = "ThisName" };
            var class2 = new Class1() { Id = 1, Name = "This Name" };

            //When
            var assert = () => class1.Should().BeEquivalentTo(class2, o => o.WithPropertyAssertion<Class1, string>( c => c.Subject.Should().Be(c.Expectation)));

            //Then
            assert.Should().NotThrow();
        }

        class Class1
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
