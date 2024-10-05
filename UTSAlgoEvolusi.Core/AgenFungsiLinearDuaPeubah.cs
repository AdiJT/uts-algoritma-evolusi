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

public class AgenBinaryResult
{
    public Kromoson GlobalBest { get; }
    public List<Kromoson> LocalBests { get; }
    public double KonvergensiPopulasi { get; }
    public int CounterGenerasi { get; }

    public AgenBinaryResult(
        Kromoson globalBest, 
        List<Kromoson> localBest, 
        double konvergensiPopulasi, 
        int counterGenerasi)
    {
        GlobalBest = globalBest;
        LocalBests = localBest;
        KonvergensiPopulasi = konvergensiPopulasi;
        CounterGenerasi = counterGenerasi;
    }
}

public class AgenFungsiLinearDuaPeubah
{
    private (double bawah, double atas) _batasX;
    private (double bawah, double atas) _batasY;

    public JenisAgen JenisAgen { get; set; } = JenisAgen.Min;
    public int JumlahGenerasi { get; set; } = 1000;
    public int JumlahPopulasi { get; set; } = 10;
    public double ProbabilitasCrossover { get; set; } = 0.50;
    public double ProbabilitasMutasi { get; set; } = 0.1;
    public double BatasKonvergensiPopulasi { get; set; } = 0.8;
    public Func<LinearDuaPeubah, double> FungsiObjektif { get; set; }

    public ISeleksi Seleksi { get; set; }
    public ICrossover Crossover { get; set; }
    public IEncoder<int, LinearDuaPeubah> Encoder { get; set; }
    public IDecoder<int, LinearDuaPeubah> Decoder { get; set; }

    public (double bawah, double atas) BatasX
    {
        get => _batasX;
        set
        {
            if (value.bawah > value.atas)
                throw new ArgumentException("BatasX.bawah lebih dari BatasX.atas");

            _batasX = value;
        }
    }

    public (double bawah, double atas) BatasY
    {
        get => _batasY;
        set
        {
            if (value.bawah > value.atas)
                throw new ArgumentException("BatasY.bawah lebih dari BatasY.atas");

            _batasY = value;
        }
    }

    public AgenFungsiLinearDuaPeubah(
        Func<LinearDuaPeubah, double> fungsiObjektif,
        (double bawah, double atas) batasX,
        (double bawah, double atas) batasY,
        ISeleksi seleksi,
        ICrossover crossover,
        IEncoder<int, LinearDuaPeubah> encoder,
        IDecoder<int, LinearDuaPeubah> decoder)
    {
        FungsiObjektif = fungsiObjektif;
        BatasX = batasX;
        BatasY = batasY;
        Seleksi = seleksi;
        Crossover = crossover;
        Encoder = encoder;
        Decoder = decoder;
    }

    public AgenBinaryResult Execute()
    {
        var counterGenerasi = -1;
        var populasi = GeneratePopulasiAwal();
        var localBests = new List<Kromoson>();
        Kromoson? globalBest = null;
        double globalBestFitness = JenisAgen == JenisAgen.Max ? double.MinValue : double.MaxValue;
        var random = new Random();

        while(counterGenerasi < JumlahGenerasi && !IsPopulasiKonvergen(populasi))
        {
            //Perhitungan Fitness dan Penentuan Local dan Global Best
            var fitnessPopulasi = HitungFitnessPopulasi(populasi);

            var localBestFitness = JenisAgen == JenisAgen.Max ? fitnessPopulasi.Max() : fitnessPopulasi.Min();
            var localBestIndex = fitnessPopulasi.IndexOf(localBestFitness);
            var localBest = new Kromoson(populasi[localBestIndex]);

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
            populasi = Seleksi.Seleksi(this, populasi);

            //Kawin Silang
            populasi = KawinSilang(populasi);

            //Mutasi
            populasi = Mutasi(populasi);

            counterGenerasi++;
        }

        var hasil = new AgenBinaryResult(globalBest!, localBests, HitungKonvergensiPopulasi(populasi), counterGenerasi);

        return hasil;
    }

    private List<Kromoson> KawinSilang(List<Kromoson> populasi)
    {
        var newPopulasi = populasi.Select(k => new Kromoson(k)).ToList();
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
                var hasilKawin = Crossover.Crossover(this, kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[i + 1]]);
                newPopulasi[indexKawinSilang[i]] = hasilKawin.anak1;
                newPopulasi[indexKawinSilang[i + 1]] = hasilKawin.anak2;
            }
            else
            {
                var parent2Index = random.GetItem(indexKawinSilang.Take(indexKawinSilang.Length - 1));
                var hasilKawin = Crossover.Crossover(this, kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[parent2Index]]);
                newPopulasi[indexKawinSilang[i]] = hasilKawin.anak1;
            }
        }

        return newPopulasi;
    }

    public List<double> HitungFitnessPopulasi(List<Kromoson> populasi) => populasi.Select(k => FungsiObjektif(Decoder.Decode(k.Gen)) ).ToList();

    public List<Kromoson> GeneratePopulasiAwal()
    {
        var populasiAwal = new List<Kromoson>();
        var random = new Random();

        for (var i = 0; i < JumlahPopulasi; i++)
        {
            var randX = random.NextDouble(BatasX.bawah, BatasX.atas);
            var randY = random.NextDouble(BatasY.bawah, BatasY.atas);
            populasiAwal.Add(new Kromoson(Encoder.Encode(new LinearDuaPeubah { X = randX, Y = randY })));
        }

        return populasiAwal;
    }

    public bool IsPopulasiKonvergen(List<Kromoson> populasi)
    {
        var konvergensi = HitungKonvergensiPopulasi(populasi);

        return konvergensi > BatasKonvergensiPopulasi;
    }

    public double HitungKonvergensiPopulasi(List<Kromoson> populasi)
    {
        var evaluasi = HitungFitnessPopulasi(populasi);

        var hashSet = new HashSet<double>();
        var jumlahIndividuSama = 1;

        foreach(var eval in evaluasi)
            if (!hashSet. Add(eval))
                jumlahIndividuSama++;

        return jumlahIndividuSama / (double)populasi.Count;
    }

    public List<Kromoson> Mutasi(List<Kromoson> populasi)
    {
        var newPopulasi = new List<Kromoson>();
        var random = new Random();

        foreach (var kromoson in populasi)
        {
            var newKromoson = new Kromoson(kromoson);
            for (var i = 0; i < newKromoson.PanjangGen; i++)
            {
                var p = random.NextDouble();
                if (p <= ProbabilitasMutasi)
                    newKromoson[i] = newKromoson[i] == 1 ? 0 : 1;
            }

            newPopulasi.Add(newKromoson);
        }

        return newPopulasi;
    }
}
