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

public class AgenResult
{
    public Kromoson<int> GlobalBest { get; }
    public List<List<Kromoson<int>>> PopulasiPerGenerasi { get; }
    public List<Kromoson<int>> LocalBests { get; }
    public double KonvergensiPopulasi { get; }
    public int CounterGenerasi { get; }

    public AgenResult(
        Kromoson<int> globalBest,
        List<Kromoson<int>> localBest,
        List<List<Kromoson<int>>> populasiPerGenerasi,
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

public class Agen
{
    public JenisAgen JenisAgen { get; set; } = JenisAgen.Min;
    public int JumlahGenerasi { get; set; } = 1000;
    public int JumlahPopulasi { get; set; } = 10;
    public double ProbabilitasCrossover { get; set; } = 0.50;
    public double ProbabilitasMutasi { get; set; } = 0.1;
    public double BatasKonvergensiPopulasi { get; set; } = 0.8;

    public Func<LinearDuaPeubah, double> FungsiObjektif { get; set; }

    public ISeleksi<int, LinearDuaPeubah> Seleksi { get; set; }
    public ICrossover<int> Crossover { get; set; }
    public IEncoding<int, LinearDuaPeubah> Encoding { get; set;}

    public Agen(
        Func<LinearDuaPeubah, double> fungsiObjektif,
        ISeleksi<int, LinearDuaPeubah> seleksi,
        IEncoding<int, LinearDuaPeubah> encoding,
        ICrossover<int> crossover,
        (double bawah, double atas) batasX,
        (double bawah, double atas) batasY)
    {
        FungsiObjektif = fungsiObjektif;
        Seleksi = seleksi;
        Crossover = crossover;
        Encoding = encoding;
    }

    public AgenResult Execute(List<Kromoson<int>> populasiAwal)
    {
        var populasi = populasiAwal.Select(k => new Kromoson<int>(k)).ToList();

        var counterGenerasi = -1;

        var localBests = new List<Kromoson<int>>();
        Kromoson<int>? globalBest = null;
        var populasiPerGenerasi = new List<List<Kromoson<int>>>();
        double globalBestFitness = JenisAgen == JenisAgen.Max ? double.MinValue : double.MaxValue;
        var random = new Random();

        while (counterGenerasi < JumlahGenerasi && !IsPopulasiKonvergen(populasi))
        {
            //Perhitungan Fitness dan Penentuan Local dan Global Best
            var fitnessPopulasi = HitungFitnessPopulasi(populasi);

            var localBestFitness = JenisAgen == JenisAgen.Max ? fitnessPopulasi.Max() : fitnessPopulasi.Min();
            var localBestIndex = fitnessPopulasi.IndexOf(localBestFitness);
            var localBest = new Kromoson<int>(populasi[localBestIndex]);

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

            populasiPerGenerasi.Add(populasi.Select(k => new Kromoson<int>(k)).ToList());

            counterGenerasi++;
        }

        var hasil = new AgenResult(globalBest!, localBests, populasiPerGenerasi, HitungKonvergensiPopulasi(populasi), counterGenerasi);

        return hasil;
    }

    private List<Kromoson<int>> KawinSilang(List<Kromoson<int>> populasi)
    {
        var newPopulasi = populasi.Select(k => new Kromoson<int>(k)).ToList();
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

    public List<double> HitungFitnessPopulasi(List<Kromoson<int>> populasi) => populasi.Select(k => FungsiObjektif(Encoding.Decode(k))).ToList();

    public bool IsPopulasiKonvergen(List<Kromoson<int>> populasi)
    {
        var konvergensi = HitungKonvergensiPopulasi(populasi);

        return konvergensi > BatasKonvergensiPopulasi;
    }

    public double HitungKonvergensiPopulasi(List<Kromoson<int>> populasi)
    {
        var evaluasi = HitungFitnessPopulasi(populasi);

        var hashSet = new HashSet<double>();
        var jumlahIndividuSama = 1;

        foreach(var eval in evaluasi)
            if (!hashSet. Add(eval))
                jumlahIndividuSama++;

        return jumlahIndividuSama / (double)populasi.Count;
    }

    public List<Kromoson<int>> Mutasi(List<Kromoson<int>> populasi)
    {
        var newPopulasi = new List<Kromoson<int>>();
        var random = new Random();

        foreach (var kromoson in populasi)
        {
            var newKromoson = new Kromoson<int>(kromoson);
            for (var i = 0; i < newKromoson.DaftarAlel.Count; i++)
            {
                var p = random.NextDouble();
                if (p <= ProbabilitasMutasi)
                    newKromoson.DaftarAlel[i] = newKromoson.DaftarAlel[i] == 1 ? 0 : 1;
            }

            newPopulasi.Add(newKromoson);
        }

        return newPopulasi;
    }
}
