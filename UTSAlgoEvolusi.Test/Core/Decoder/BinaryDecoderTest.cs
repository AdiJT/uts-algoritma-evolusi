using FluentAssertions;
using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Decoder;

namespace UTSAlgoEvolusi.Test.Core.Decoder;

public class BinaryDecoderTest
{
    public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[]
            {
                (-5.12, 5.12),
                (-5.12, 5.12),
                4,
                new List<int>{ 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                new LinearDuaPeubah{ X = -4.9052, Y=  1.7551 }
            }
        };

    [Theory]
    [MemberData(nameof(Data))]
    public void Decode_Should_ReturnExcepted(
        (double bawah, double atas) batasX,
        (double bawah, double atas) batasY,
        int presisi,
        List<int> encoding,
        LinearDuaPeubah expected)
    {
        //Arrange
        var decoder = new BinaryDecoder
        {
            BatasX = batasX,
            BatasY = batasY,
            Presisi = presisi
        };
        expected.X = Math.Round(expected.X, presisi);
        expected.Y = Math.Round(expected.Y, presisi);

        //Act
        var result = decoder.Decode(encoding);
        result.X = Math.Round(result.X, presisi);
        result.Y = Math.Round(result.Y, presisi);

        //Assert
        result.Should().BeEquivalentTo(expected);
    }
}
