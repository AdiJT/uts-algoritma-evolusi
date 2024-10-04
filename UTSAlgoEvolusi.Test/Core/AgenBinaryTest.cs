using FluentAssertions;
using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Seleksi;

namespace UTSAlgoEvolusi.Test.Core;

public class AgenBinaryTest
{
    [Fact]
    public void Execute_Should_GlobalBestEqualToBestOfLocalBest()
    {
        //Arrange
        var agen = new AgenBinary(
            (x, y) => (x * x - 10 * Math.Cos(2 * Math.PI * x)) + (y * y - 10 * Math.Cos(2 * Math.PI * y)) + 20,
            (-5.12, 5.12),
            (-5.12, 5.12),
            new RouletteWheel(),
            new SinglePointCrossover())
        {
            JumlahGenerasi = 1000,
            JumlahPopulasi = 10,
            ProbabilitasCrossover = 0.25,
            ProbabilitasMutasi = 0.01,
            BatasKonvergensiPopulasi = 0.9,
            JenisAgen = JenisAgen.Min
        };

        //Act
        var result = agen.Execute();

        //Assert
        result.Should().NotBeNull();
        var fitnessLocalBests = agen.HitungFitnessPopulasi(result.LocalBests);
        var bestOfLocalBestFitness = fitnessLocalBests.Min();
        var bestOfLocalBestIndex = fitnessLocalBests.IndexOf(bestOfLocalBestFitness);
        var bestOfLocalBest = result.LocalBests[bestOfLocalBestIndex];

        result.GlobalBest.Should().BeEquivalentTo(bestOfLocalBest);
    }

    [Fact]
    public void Execute_Should_GlobalBestDecodeMustBetweenBatasX_And_BatasY()
    {
        //Arrange
        (double bawah, double atas) batasXFungsiObjektif = (-5.12, 5.12);
        (double bawah, double atas) batasYFungsiObjektif = (-5.12, 5.12);
        var agen = new AgenBinary(
            (x, y) => (x * x - 10 * Math.Cos(2 * Math.PI * x)) + (y * y - 10 * Math.Cos(2 * Math.PI * y)) + 20,
            batasXFungsiObjektif,
            batasYFungsiObjektif,
            new RouletteWheel(),
            new SinglePointCrossover())
        {
            JumlahGenerasi = 1000,
            JumlahPopulasi = 10,
            ProbabilitasCrossover = 0.25,
            ProbabilitasMutasi = 0.01,
            BatasKonvergensiPopulasi = 0.9,
            JenisAgen = JenisAgen.Min
        };

        //Act
        var result = agen.Execute();

        //Assert
        result.Should().NotBeNull();

        var decodeGlobalBest = result.GlobalBest.Decoding();
        var fitnessGlobalBest = agen.FungsiObjektif(decodeGlobalBest.x, decodeGlobalBest.y);

        decodeGlobalBest.x.Should().BeInRange(batasXFungsiObjektif.bawah, batasXFungsiObjektif.atas);
        decodeGlobalBest.y.Should().BeInRange(batasYFungsiObjektif.bawah, batasYFungsiObjektif.atas);
    }
}
