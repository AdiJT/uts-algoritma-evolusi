using FluentAssertions;
using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Encoder;

namespace UTSAlgoEvolusi.Test.Core.Encoder;

public class BinaryEncoderTest
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
    public void Encode_Should_ReturnExcepted(
        (double bawah, double atas) batasX,
        (double bawah, double atas) batasY,
        int presisi,
        List<int> expected,
        LinearDuaPeubah asli)
    {
        //Arrange
        var encoder = new BinaryEncoder
        {
            BatasX = batasX,
            BatasY = batasY,
            Presisi = presisi,
        };

        //Act
        var encoding = encoder.Encode(asli);

        //Assert
        encoding.Should().Equal(expected);
    }
}
