using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Seleksi;

public class RouletteWheel<TAlel, TAsli> : ISeleksi<TAlel, TAsli>
{
    public List<Kromoson<TAlel>> Seleksi(
        List<Kromoson<TAlel>> populasi, 
        IEncoding<TAlel, TAsli> encoding, 
        Func<TAsli, double> fungsiObjektif, 
        JenisAgen jenis)
    {
        var evalFunc = jenis == JenisAgen.Max ? fungsiObjektif : arg => 1 / fungsiObjektif(arg);
        var daftarEvaluasi = populasi.Select(k => evalFunc(encoding.Decode(k))).ToList();

        var total = daftarEvaluasi.Sum();
        var probabilitasKumulatif = new List<double>();
        var totalKumulatif = 0d;
        var random = new Random();

        foreach (var eval in daftarEvaluasi)
        {
            var probabilitas = eval / total;
            totalKumulatif += probabilitas;
            probabilitasKumulatif.Add(totalKumulatif);
        }

        var populasiBaru = new List<Kromoson<TAlel>>();

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
