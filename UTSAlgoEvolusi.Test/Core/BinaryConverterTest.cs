using FluentAssertions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Test.Core
{
    public class BinaryConverterTest
    {
        public static IEnumerable<object[]> ToBinaryTestData => new List<object[]>
        {
            new object[] { 0, 3, new List<int> {0, 0, 0} },
            new object[] { 1, 3, new List<int> {0, 0, 1} },
            new object[] { 2, 3, new List<int> {0, 1, 0} },
            new object[] { 3, 3, new List<int> {0, 1, 1} },
            new object[] { 4, 3, new List<int> {1, 0, 0} },
            new object[] { 5, 3, new List<int> {1, 0, 1} },
            new object[] { 6, 3, new List<int> {1, 1, 0} },
            new object[] { 7, 3, new List<int> {1, 1, 1} },
        };

        [Theory]
        [MemberData(nameof(ToBinaryTestData))]
        public void ToBinary_Should_ReturnCorrectBinary(int number, int maksDigit ,List<int> expected)
        {
            //Act
            var result = BinaryConverter.ToBinary(number, maksDigit);

            //Assert
            result.Count.Should().Be(maksDigit);
            result.Should().Equal(expected);
        }
    }
}
