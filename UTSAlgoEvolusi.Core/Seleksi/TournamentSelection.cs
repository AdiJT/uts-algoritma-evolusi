using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Core.Seleksi;

public class TournamentSelection<TAlel, TAsli> : ISeleksi<TAlel, TAsli>
{
    public List<Kromoson<TAlel>> Seleksi(
        List<Kromoson<TAlel>> populasi, 
        IEncoding<TAlel, TAsli> encoding, 
        Func<TAsli, double> fungsiObjektif, 
        JenisAgen jenis)
    {
        var hasilEvaluasi = populasi.Select(k => fungsiObjektif(encoding.Decode(k))).ToList();
        var populasiBaru = new List<Kromoson<TAlel>>();
        var random = new Random();
        Func<double, double, bool> compareFunc = jenis == JenisAgen.Max ? (double f1, double f2) => f1 >= f2 : (double f1, double f2) => f1 <= f2;

        for(int i = 0; i <  populasi.Count; i++)
        {
            var kromoson1Index = random.Next(0, populasi.Count);
            var kromoson2Index = random.Next(0, populasi.Count, kromoson1Index);

            var fitness1 = hasilEvaluasi[kromoson1Index];
            var fitness2 = hasilEvaluasi[kromoson2Index];

            if (compareFunc(fitness1, fitness2))
                populasiBaru.Add(new Kromoson<TAlel>(populasi[kromoson1Index]));
            else
                populasiBaru.Add(new Kromoson<TAlel>(populasi[kromoson2Index]));
        }

        return populasiBaru;
    }
}
