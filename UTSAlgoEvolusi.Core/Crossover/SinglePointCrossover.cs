using System;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Crossover;

public class SinglePointCrossover : ICrossover
{
    public (Kromoson anak1, Kromoson anak2) Crossover(AgenBinary agen, Kromoson parent1, Kromoson parent2)
    {
        var random = new Random();
        var titikPotong = random.Next(0, parent1.JumlahGen);

        var anak1 = new Kromoson(agen.BatasXFungsiObjektif, agen.BatasYFungsiObjektif, agen.Presisi);
        var anak2 = new Kromoson(agen.BatasXFungsiObjektif, agen.BatasYFungsiObjektif, agen.Presisi);

        for(var i = 0; i < titikPotong; i++)
        {
            anak1[i] = parent1[i];
            anak2[i] = parent2[i];
        }

        for (var i = titikPotong; i < parent1.JumlahGen; i++)
        {
            anak1[i] = parent2[i];
            anak2[i] = parent1[i];
        }

        return (anak1, anak2);
    }
}
