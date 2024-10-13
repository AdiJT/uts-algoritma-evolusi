using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Core;

public enum JenisAgen
{
    Max, Min
}

public class AgenResult<TAlel>
{
    public Kromoson<TAlel> GlobalBest { get; }
    public List<List<Kromoson<TAlel>>> PopulasiPerGenerasi { get; }
    public List<Kromoson<TAlel>> LocalBests { get; }
    public double KonvergensiPopulasi { get; }
    public int CounterGenerasi { get; }

    public AgenResult(
        Kromoson<TAlel> globalBest,
        List<Kromoson<TAlel>> localBest,
        List<List<Kromoson<TAlel>>> populasiPerGenerasi,
        double konvergensiPopulasi,
        int counterGenerasi)
    {
        GlobalBest = globalBest;
        LocalBests = localBest;
        KonvergensiPopulasi = konvergensiPopulasi;
        CounterGenerasi = counterGenerasi;
        PopulasiPerGenerasi = populasiPerGenerasi;
    }
}

public class Agen<TAlel, TAsli>
{
    private readonly CustomDoubleEqualityComparer _customDoubleEqualityComparer = new(1e-7);

    public JenisAgen JenisAgen { get; set; } = JenisAgen.Min;
    public int JumlahGenerasi { get; set; } = 1000;
    public int JumlahPopulasi { get; set; } = 10;
    public double ProbabilitasCrossover { get; set; } = 0.50;
    public double ProbabilitasMutasi { get; set; } = 0.1;
    public double BatasKonvergensiPopulasi { get; set; } = 0.8;

    public Func<TAsli, double> FungsiObjektif { get; set; }

    public ISeleksi<TAlel, TAsli> Seleksi { get; set; }
    public ICrossover<TAlel> Crossover { get; set; }
    public IEncoding<TAlel, TAsli> Encoding { get; set;}

    public Agen(
        Func<TAsli, double> fungsiObjektif,
        ISeleksi<TAlel, TAsli> seleksi,
        IEncoding<TAlel, TAsli> encoding,
        ICrossover<TAlel> crossover)
    {
        FungsiObjektif = fungsiObjektif;
        Seleksi = seleksi;
        Crossover = crossover;
        Encoding = encoding;
    }

    public AgenResult<TAlel> Execute(List<Kromoson<TAlel>> populasiAwal)
    {
        if (populasiAwal.Count != JumlahPopulasi)
            throw new ArgumentException("jumlah populasiAwal tidak sama dengan JumlahPopulasi");

        var populasi = populasiAwal.Select(k => new Kromoson<TAlel>(k)).ToList();

        var counterGenerasi = 0;
        var populasiPerGenerasi = new List<List<Kromoson<TAlel>>>();
        var localBests = new List<Kromoson<TAlel>>();
        Kromoson<TAlel>? globalBest = null;
        double globalBestFitness = JenisAgen == JenisAgen.Max ? double.MinValue : double.MaxValue;

        var random = new Random();

        while (counterGenerasi < JumlahGenerasi && !IsPopulasiKonvergen(populasi))
        {
            //Perhitungan Fitness dan Penentuan Local dan Global Best
            var fitnessPopulasi = HitungFitnessPopulasi(populasi);

            var localBestFitness = JenisAgen == JenisAgen.Max ? fitnessPopulasi.Max() : fitnessPopulasi.Min();
            var localBestIndex = fitnessPopulasi.IndexOf(localBestFitness);
            var localBest = new Kromoson<TAlel>(populasi[localBestIndex]);

            localBests.Add(localBest);
            if (globalBest is null)
            {
                globalBest = localBest;
                globalBestFitness = localBestFitness;
            }
            else
            {
                Func<double, double, bool> compare = JenisAgen == JenisAgen.Max ? (double gb, double lb) => gb < lb : (double gb, double lb) => gb > lb;
                if (compare(globalBestFitness, localBestFitness))
                {
                    globalBest = localBest;
                    globalBestFitness = localBestFitness;
                }
            }

            //Seleksi
            populasi = Seleksi.Seleksi(populasi, Encoding, FungsiObjektif, JenisAgen);

            //Kawin Silang
            populasi = KawinSilang(populasi);

            //Mutasi
            populasi = Mutasi(populasi);

            populasiPerGenerasi.Add(populasi.Select(k => new Kromoson<TAlel>(k)).ToList());

            counterGenerasi++;
        }

        var hasil = new AgenResult<TAlel>(globalBest!, localBests, populasiPerGenerasi, HitungKonvergensiPopulasi(populasi), counterGenerasi);

        return hasil;
    }

    private List<Kromoson<TAlel>> KawinSilang(List<Kromoson<TAlel>> populasi)
    {
        var newPopulasi = populasi.Select(k => new Kromoson<TAlel>(k)).ToList();
        var random = new Random();

        var daftarProbabilitasCrossover = Enumerable.Range(1, JumlahPopulasi).Select(x => random.NextDouble());
        while (daftarProbabilitasCrossover.Count(x => x <= ProbabilitasCrossover) < 2)
            daftarProbabilitasCrossover = Enumerable.Range(1, JumlahPopulasi).Select(x => random.NextDouble());

        var indexKawinSilang = daftarProbabilitasCrossover.Select((x, index) => x <= ProbabilitasCrossover ? index : 0).TakeWhile(x => x != 0).ToArray();
        var kandidatKawinSilang = indexKawinSilang.ToDictionary(i => i, i => newPopulasi[i]);

        random.Shuffle(indexKawinSilang);

        for(var i = 0; i < indexKawinSilang.Length; i += 2)
        {
            if (i != indexKawinSilang.Length - 1)
            {
                var hasilKawin = Crossover.Crossover(kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[i + 1]]);
                newPopulasi[indexKawinSilang[i]] = hasilKawin.anak1;
                newPopulasi[indexKawinSilang[i + 1]] = hasilKawin.anak2;
            }
            else
            {
                var parent2Index = random.GetItem(indexKawinSilang.Take(indexKawinSilang.Length - 1));
                var hasilKawin = Crossover.Crossover(kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[parent2Index]]);
                newPopulasi[indexKawinSilang[i]] = hasilKawin.anak1;
            }
        }

        return newPopulasi;
    }

    public List<double> HitungFitnessPopulasi(List<Kromoson<TAlel>> populasi) => populasi.Select(k => FungsiObjektif(Encoding.Decode(k))).ToList();

    public bool IsPopulasiKonvergen(List<Kromoson<TAlel>> populasi)
    {
        var konvergensi = HitungKonvergensiPopulasi(populasi);

        return konvergensi > BatasKonvergensiPopulasi;
    }

    public double HitungKonvergensiPopulasi(List<Kromoson<TAlel>> populasi)
    {
        var evaluasi = HitungFitnessPopulasi(populasi);

        var hashSet = new HashSet<double>(_customDoubleEqualityComparer);
        var jumlahIndividuSama = 1;

        foreach(var eval in evaluasi)
            if (!hashSet. Add(eval))
                jumlahIndividuSama++;

        return jumlahIndividuSama / (double)populasi.Count;
    }

    public List<Kromoson<TAlel>> Mutasi(List<Kromoson<TAlel>> populasi)
    {
        var newPopulasi = new List<Kromoson<TAlel>>();
        var random = new Random();

        foreach (var kromoson in populasi)
        {
            var newKromoson = new Kromoson<TAlel>(kromoson);
            for (var i = 0; i < newKromoson.DaftarAlel.Count; i++)
            {
                var p = random.NextDouble();
                if (p <= ProbabilitasMutasi)
                    newKromoson = Encoding.Mutasi(kromoson, i);
            }

            newPopulasi.Add(newKromoson);
        }

        return newPopulasi;
    }
}
