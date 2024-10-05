using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Decoder;
using UTSAlgoEvolusi.Core.Encoder;
using UTSAlgoEvolusi.Core.Seleksi;

namespace UTSAlgoEvolusi.Test.Core;

public class AgenFungsiLinearDuaPeubahTest
{
    [Fact]
    public void Execute_ReturnGlobalEqualToExpected()
    {
        //Arrange
        (double bawah, double atas) batasX = (-5.12, 5.12);
        (double bawah, double atas) batasY = (-5.12, 5.12);
        const int presisi = 4;
        var seleksi = new RouletteWheel();
        var crossover = new SinglePointCrossover();

        var encoder = new BinaryEncoder
        {
            BatasX = batasX,
            BatasY = batasY,
            Presisi = presisi
        };

        var decoder = new BinaryDecoder
        {
            BatasX = batasX,
            BatasY = batasY,
            Presisi = presisi
        };

        var agen = new AgenFungsiLinearDuaPeubah(
            arg => (arg.X * arg.X - 10 * Math.Cos(Math.Tau * arg.X)) + (arg.Y * arg.Y - 10 * Math.Cos(Math.Tau * arg.Y)) + 20,
            batasX,
            batasY,
            seleksi,
            crossover,
            encoder,
            decoder);

        //Act

        //Assert

    }
}
