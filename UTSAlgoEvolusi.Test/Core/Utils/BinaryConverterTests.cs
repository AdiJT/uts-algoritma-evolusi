using FluentAssertions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Test.Core.Utils
{
    public class BinaryConverterTests
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
            new object[] { 0, 4, new List<int> {0, 0, 0, 0} },
            new object[] { 1, 4, new List<int> {0, 0, 0, 1} },
            new object[] { 2, 4, new List<int> {0, 0, 1, 0} },
            new object[] { 3, 4, new List<int> {0, 0, 1, 1} },
            new object[] { 4, 4, new List<int> {0, 1, 0, 0} },
            new object[] { 5, 4, new List<int> {0, 1, 0, 1} },
            new object[] { 6, 4, new List<int> {0, 1, 1, 0} },
            new object[] { 7, 4, new List<int> {0, 1, 1, 1} },
            new object[] { 8, 4, new List<int> {1, 0, 0, 0} },
            new object[] { 9, 4, new List<int> {1, 0, 0, 1} },
            new object[] { 10, 4, new List<int> {1, 0, 1, 0} },
            new object[] { 11, 4, new List<int> {1, 0, 1, 1} },
            new object[] { 12, 4, new List<int> {1, 1, 0, 0} },
            new object[] { 13, 4, new List<int> {1, 1, 0, 1} },
            new object[] { 14, 4, new List<int> {1, 1, 1, 0} },
            new object[] { 15, 4, new List<int> {1, 1, 1, 1} },
            new object[] { 2750, 17, new List<int> { 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0 } },
            new object[] { 88000, 17, new List<int> { 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } }
        };

        [Theory]
        [MemberData(nameof(ToBinaryTestData))]
        public void ToBinary_Should_ReturnCorrectBinary(int number, int maksDigit, List<int> expected)
        {
            //Act
            var result = BinaryConverter.ToBinary(number, maksDigit);

            //Assert
            result.Count.Should().Be(maksDigit);
            result.Should().Equal(expected);
        }

        [Theory]
        [MemberData(nameof(ToBinaryTestData))]
        public void ToInt_Should_ReturnCorrectNumber(int expected, int maksDigit, List<int> binary)
        {
            //Act
            var result = BinaryConverter.ToInt(binary);

            //Assert
            result.Should().Be(expected);
        }
    }
}
