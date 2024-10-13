using System;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Crossover;

public class SinglePointCrossover<TAlel> : ICrossover<TAlel>
{
    public (Kromoson<TAlel> anak1, Kromoson<TAlel> anak2) Crossover(Kromoson<TAlel> parent1, Kromoson<TAlel> parent2)
    {
        if (parent1.DaftarAlel.Count != parent2.DaftarAlel.Count)
            throw new ArgumentException("Panjang kromoson parent1 dan parent2 tidak sama");

        var random = new Random();
        var jumlahAlel = parent1.DaftarAlel.Count;
        var titikPotong = random.Next(0, jumlahAlel);

        var anak1 = new Kromoson<TAlel>(jumlahAlel);
        var anak2 = new Kromoson<TAlel>(jumlahAlel);

        for(var i = 0; i < titikPotong; i++)
        {
            anak1.DaftarAlel[i] = parent1.DaftarAlel[i];
            anak2.DaftarAlel[i] = parent2.DaftarAlel[i];
        }

        for (var i = titikPotong; i < jumlahAlel; i++)
        {
            anak1.DaftarAlel[i] = parent2.DaftarAlel[i];
            anak2.DaftarAlel[i] = parent1.DaftarAlel[i];
        }

        return (anak1, anak2);
    }
}
