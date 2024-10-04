using System;
using System.Collections;
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

public class AgenBinary
{
    private (double bawah, double atas) _batasXFungsiObjektif;
    private (double bawah, double atas) _batasYFungsiObjektif;
    private int _presisi = 4;

    public JenisAgen JenisAgen { get; set; } = JenisAgen.Min;
    public int JumlahGenerasi { get; set; } = 1000;
    public int JumlahPopulasi { get; set; } = 10;
    public double ProbabilitasCrossover { get; set; } = 0.50;
    public double ProbabilitasMutasi { get; set; } = 0.1;
    public double BatasKonvergensiPopulasi { get; set; } = 0.8;
    public Func<double, double, double> FungsiObjektif { get; set; }

    public (double bawah, double atas) BatasXFungsiObjektif 
    { 
        get => _batasXFungsiObjektif; 
        set
        {
            if (value.bawah > value.atas)
                throw new ArgumentException("Batas X Bawah melebihi Batas X Atas");

            _batasXFungsiObjektif = value; 
        } 
    }

    public (double bawah, double atas) BatasYFungsiObjektif 
    { 
        get => _batasYFungsiObjektif; 
        set 
        {
            if (value.bawah > value.atas)
                throw new ArgumentException("Batas Y Bawah melebihi Batas Y Atas");

            _batasYFungsiObjektif = value;
        }
    }

    public int Presisi 
    { 
        get => _presisi; 
        set 
        {
            if (value <= 0)
                throw new ArgumentException("Presisi tidak boleh 0 atau negatif");

            _presisi = value;
        } 
    }
    public ISeleksi Seleksi { get; set; }
    public ICrossover Crossover { get; set; }

    public AgenBinary(
        Func<double, double, double> fungsiObjektif, 
        (double bawah, double atas) batasXFungsiObjektif, 
        (double bawah, double atas) batasYFungsiObjektif, 
        ISeleksi seleksi, 
        ICrossover crossover)
    {
        FungsiObjektif = fungsiObjektif;
        BatasXFungsiObjektif = batasXFungsiObjektif;
        BatasYFungsiObjektif = batasYFungsiObjektif;
        Seleksi = seleksi;
        Crossover = crossover;
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
            var localBest = populasi[localBestIndex];

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
        var random = new Random();

        var daftarProbabilitasCrossover = Enumerable.Range(1, JumlahPopulasi).Select(x => random.NextDouble());
        while (daftarProbabilitasCrossover.Count(x => x <= ProbabilitasCrossover) < 2)
            daftarProbabilitasCrossover = Enumerable.Range(1, JumlahPopulasi).Select(x => random.NextDouble());

        var indexKawinSilang = daftarProbabilitasCrossover.Select((x, index) => x <= ProbabilitasCrossover ? index : 0).TakeWhile(x => x != 0).ToArray();
        var kandidatKawinSilang = indexKawinSilang.ToDictionary(i => i, i => populasi[i]);

        random.Shuffle(indexKawinSilang);

        for(var i = 0; i < indexKawinSilang.Length; i += 2)
        {
            if (i != indexKawinSilang.Length - 1)
            {
                var hasilKawin = Crossover.Crossover(this, kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[i + 1]]);
                populasi[indexKawinSilang[i]] = hasilKawin.anak1;
                populasi[indexKawinSilang[i + 1]] = hasilKawin.anak2;
            }
            else
            {
                var parent2Index = random.GetItem(indexKawinSilang.Take(indexKawinSilang.Length - 1));
                var hasilKawin = Crossover.Crossover(this, kandidatKawinSilang[indexKawinSilang[i]], kandidatKawinSilang[indexKawinSilang[parent2Index]]);
                populasi[indexKawinSilang[i]] = hasilKawin.anak1;
            }
        }

        return populasi;
    }

    public List<double> HitungFitnessPopulasi(List<Kromoson> populasi) => populasi.Select(k => 
    {
        var decode = k.Decoding();
        return FungsiObjektif(decode.x, decode.y);
    }).ToList();

    public List<Kromoson> GeneratePopulasiAwal()
    {
        var populasiAwal = new List<Kromoson>();
        var random = new Random();

        for(var i = 0; i < JumlahPopulasi; i++)
        {
            var randX = random.NextDouble(BatasXFungsiObjektif.bawah, BatasXFungsiObjektif.atas);
            var randY = random.NextDouble(BatasYFungsiObjektif.bawah, BatasYFungsiObjektif.atas);
            populasiAwal.Add(Kromoson.Encoding(BatasXFungsiObjektif, BatasYFungsiObjektif, Presisi, randX, randY));
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
        var random = new Random();

        for(var i = 0; i < populasi.Count; i++)
        {
            for(var j = 0; j < populasi[i].JumlahGen; j++)
            {
                var p = random.NextDouble();
                if (p <= ProbabilitasMutasi)
                    populasi[i].Gen[j] = populasi[i][j] == 0 ? 1 : 0;
            }
        }

        return populasi;
    }
}
