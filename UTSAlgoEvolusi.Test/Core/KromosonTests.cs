using FluentAssertions;
using UTSAlgoEvolusi.Core;

namespace UTSAlgoEvolusi.Test.Core
{
    public class KromosonTests
    {
        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[]
            {
                (-5.12, 5.12),
                (-5.12, 5.12),
                4,
                new List<int>{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                (-4.9052, 1.7551)
            }
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void Decoding_Should_ReturnExcepted(
            (double bawah, double atas) batasX, 
            (double bawah, double atas) batasY, 
            int presisi, 
            List<int> kromosonGen,
            (double x, double y) expected)
        {
            //Arrange
            var kromoson = new Kromoson(batasX, batasY, presisi);

            for (int i = 0; i < kromosonGen.Count; i++)
                kromoson[i] = kromosonGen[i];

            expected.x = Math.Round(expected.x, presisi);
            expected.y = Math.Round(expected.y, presisi);

            //Act
            var result = kromoson.Decoding();
            result.x = Math.Round(result.x, presisi);
            result.y = Math.Round(result.y, presisi);

            //Assert
            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Encoding_Should_ReturnDecodingEqualToReal(
            (double bawah, double atas) batasX,
            (double bawah, double atas) batasY,
            int presisi,
            List<int> _,
            (double x, double y) real)
        {
            //Act
            var kromoson = Kromoson.Encoding(batasX, batasY, presisi, real.x, real.y);
            var result = kromoson.Decoding();

            result.x = Math.Round(result.x, presisi);
            result.y = Math.Round(result.y, presisi);

            //Arrange
            result.Should().Be((Math.Round(real.x, presisi), Math.Round(real.y, presisi)));
        }
    }
}
