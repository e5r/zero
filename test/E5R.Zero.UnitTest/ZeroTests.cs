using System;

using Xunit;

namespace E5R.Zero.UnitTest
{
    public class ZeroTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("      ")]
        public void Requires_Name(string invalidName)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Zero(invalidName));

            Assert.Equal("name", ex.ParamName);
        }

        [Theory]
        [InlineData("Valid")]
        [InlineData("Valid Name")]
        [InlineData("-- *** --")]
        public void Accept_Name(string name)
        {
            var zero = new Zero(name);

            Assert.Equal(name, zero.Name);
        }
    }
}
