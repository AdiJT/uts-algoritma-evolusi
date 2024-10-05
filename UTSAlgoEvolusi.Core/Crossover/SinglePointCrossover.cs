using System;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Crossover;

public class SinglePointCrossover : ICrossover
{
    public (Kromoson anak1, Kromoson anak2) Crossover(AgenFungsiLinearDuaPeubah agen, Kromoson parent1, Kromoson parent2)
    {
        if (parent1.PanjangGen != parent2.PanjangGen)
            throw new ArgumentException("PanjangGen parent1 dan parent2 tidak sama");

        var random = new Random();
        var panjangGen = parent1.PanjangGen;
        var titikPotong = random.Next(0, panjangGen);

        var anak1 = new Kromoson(parent1.PanjangGen);
        var anak2 = new Kromoson(parent2.PanjangGen);

        for(var i = 0; i < titikPotong; i++)
        {
            anak1[i] = parent1[i];
            anak2[i] = parent2[i];
        }

        for (var i = titikPotong; i < panjangGen; i++)
        {
            anak1[i] = parent2[i];
            anak2[i] = parent1[i];
        }

        return (anak1, anak2);
    }
}
