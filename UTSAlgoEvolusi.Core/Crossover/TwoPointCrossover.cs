using System;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Crossover;

public class TwoPointCrossover<TAlel> : ICrossover<TAlel>
{
    public (Kromoson<TAlel> anak1, Kromoson<TAlel> anak2) Crossover(Kromoson<TAlel> parent1, Kromoson<TAlel> parent2)
    {
        var panjangGen = parent1.DaftarAlel.Count;

        if (panjangGen != parent2.DaftarAlel.Count)
            throw new Exception("Panjang gen parent1 dan parent2 berbeda");

        var random = new Random();
        var awalCrossover = random.Next(0, panjangGen - 1);
        var akhirCrossover = random.Next(awalCrossover, panjangGen);

        var anak1 = new Kromoson<TAlel>(panjangGen);
        var anak2 = new Kromoson<TAlel>(panjangGen);

        for (var i = 0; i < panjangGen; i++)
        {
            if(i >= awalCrossover && i <= akhirCrossover)
            {
                anak1.DaftarAlel[i] = parent2.DaftarAlel[i];
                anak2.DaftarAlel[i] = parent1.DaftarAlel[i];
            }
            else
            {
                anak1.DaftarAlel[i] = parent1.DaftarAlel[i];
                anak2.DaftarAlel[i] = parent2.DaftarAlel[i];
            }
        }

        return (anak1, anak2);
    }
}
