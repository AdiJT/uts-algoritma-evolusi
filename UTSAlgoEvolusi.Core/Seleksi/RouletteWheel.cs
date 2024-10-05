using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Seleksi;

public class RouletteWheel : ISeleksi
{
    public List<Kromoson> Seleksi(AgenFungsiLinearDuaPeubah agen, List<Kromoson> populasi)
    {
        var evalFunc = agen.JenisAgen == JenisAgen.Max ? agen.FungsiObjektif : arg => 1 / agen.FungsiObjektif(arg);
        var evaluasi = populasi.Select(k => evalFunc(agen.Decoder.Decode(k.Gen)));

        var total = evaluasi.Sum();
        var probabilitasKumulatif = new List<double>();
        var totalKumulatif = 0d;
        var random = new Random();

        foreach (var eval in evaluasi)
        {
            var probabilitas = eval / total;
            totalKumulatif += probabilitas;
            probabilitasKumulatif.Add(totalKumulatif);
        }

        var populasiBaru = new List<Kromoson>();

        for(int i = 0; i < populasi.Count; i++)
        {
            var p = random.NextDouble();
            var k = -1;

            for (var j = probabilitasKumulatif.Count - 1; j >= 0; j--)
                if (probabilitasKumulatif[j] <  p)
                {
                    k = j;
                    break;
                }

            populasiBaru.Add(populasi[k + 1]);
        }

        if (populasiBaru.Count != populasi.Count)
            throw new Exception("Jumlah populasi hasil seleksi kurang dari populasi asli");

        return populasiBaru;
    }
}
