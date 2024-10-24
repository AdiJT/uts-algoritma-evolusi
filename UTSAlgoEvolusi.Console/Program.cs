using System.Diagnostics;
using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Encoding;
using UTSAlgoEvolusi.Core.Seleksi;

internal class Program
{
    private static void Main(string[] args)
    {
        var fungsiObjektif = (LinearDuaPeubah args) => 100 * (args.X * args.X - args.Y) * (args.X * args.X - args.Y) + (1 - args.X) * (1 - args.X);

        var encoding = new FungsiLinearDuaPeubahEncoding
        {
            Presisi = 4,
            BatasX = (-2.048, 2.048),
            BatasY = (-2.048, 2.048)
        };

        Console.WriteLine("UTS Algoritma Terinspirasi Evolusi");
        Console.WriteLine("Anggota kelompok : ");
        Console.WriteLine("1. Oswaldus Putra Fernando");
        Console.WriteLine("2. Chantika Wulandari Putri");
        Console.WriteLine("3. Adi Juanito Taklal");
        Console.WriteLine("4. Albert Berliano Tapatab");

        Console.WriteLine();
        var crossover = ChoicePrompt<ICrossover<int>>(
            "Pilih Metode Kawin Silang",
            [("Single Point Crossover", new SinglePointCrossover<int>()), ("Two Point Crossover", new TwoPointCrossover<int>())],
            new TwoPointCrossover<int>(), "Two Point Crossover");

        Console.WriteLine();
        var seleksi = ChoicePrompt<ISeleksi<int, LinearDuaPeubah>>(
            "Pilih Metode Seleksi",
            [("Roullette Wheel", new RouletteWheel<int, LinearDuaPeubah>()), ("Tournament Selection", new TournamentSelection<int, LinearDuaPeubah>())],
            new TournamentSelection<int, LinearDuaPeubah>(), "Tournament Selection");

        var agen = new Agen<int, LinearDuaPeubah>(fungsiObjektif, seleksi, encoding, crossover)
        {
            JenisAgen = JenisAgen.Max,
            JumlahGenerasi = 100,
            JumlahPopulasi = 300,
            BatasKonvergensiPopulasi = 0.8,
            ProbabilitasMutasi = 0.02,
            ProbabilitasCrossover = 0.7
        };

        Console.WriteLine("\nMeminimumkan Fungsi Objektif : f(x, y) = 100(x^2 - y)^2 + (1 - x)^2");
        Console.WriteLine($"Batas: {encoding.BatasX.bawah} <= x <= {encoding.BatasX.atas} ");
        Console.WriteLine($"Batas: {encoding.BatasY.bawah} <= y <= {encoding.BatasY.atas} ");
        Console.WriteLine($"Jumlah Generasi : {agen.JumlahGenerasi}");
        Console.WriteLine($"Jumlah Populasi : {agen.JumlahPopulasi}");
        Console.WriteLine($"Probabilitas Crossover : {agen.ProbabilitasCrossover:P2}");
        Console.WriteLine($"Probabilitas Mutasi : {agen.ProbabilitasMutasi:P2}");
        Console.WriteLine($"Batas Konvergensi : {agen.BatasKonvergensiPopulasi:P2}");

        var result = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi), verbose: true);

        var globalBest = encoding.Decode(result.GlobalBest);
        Console.WriteLine($"Global Best : (x : {globalBest.X:F8}, y : {globalBest.Y:F8}), f(x, y) = {fungsiObjektif(globalBest):F8}");
        Console.WriteLine($"Generasi Global Best : {result.GenerasiGlobalBest + 1}");

        var jumlahTes = 50;
        Console.WriteLine($"\nTes Roulette Vs Tournament Selection. Jumlah Tes : {jumlahTes}");

        var daftarHasilTesRoulette = new List<AgenResult<int>>();
        var daftarHasilTesTournament = new List<AgenResult<int>>();

        agen.Seleksi = new RouletteWheel<int, LinearDuaPeubah>();
        for (int i = 0; i < jumlahTes; i++)
        {
            var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
            daftarHasilTesRoulette.Add(r);
            Console.Write("|");
        }

        agen.Seleksi = new TournamentSelection<int, LinearDuaPeubah>();
        for (int i = 0; i < jumlahTes; i++)
        {
            var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
            daftarHasilTesTournament.Add(r);
            Console.Write("|");
        }

        var rata2Roulette = new
        {
            Fitness = daftarHasilTesRoulette.Select(r => agen.FungsiObjektif(agen.Encoding.Decode(r.GlobalBest))).Average(),
            GenerasiGlobalBest = daftarHasilTesRoulette.Average(r => r.GenerasiGlobalBest),
            RunningTime = daftarHasilTesRoulette.Average(r => r.RunningTime)
        };

        var rata2Tournament = new
        {
            Fitness = daftarHasilTesTournament.Select(r => agen.FungsiObjektif(agen.Encoding.Decode(r.GlobalBest))).Average(),
            GenerasiGlobalBest = daftarHasilTesTournament.Average(r => r.GenerasiGlobalBest),
            RunningTime = daftarHasilTesTournament.Average(r => r.RunningTime)
        };

        Console.WriteLine($"\nHasil Roullette Vs Tournament Selection");
        Console.WriteLine("Roullette Wheel :");
        Console.WriteLine($"\tRata-Rata Fitness Global Best : {rata2Roulette.Fitness:F8}");
        Console.WriteLine($"\tRata-Rata Generasi Global Best : {rata2Roulette.GenerasiGlobalBest}");
        Console.WriteLine($"\tRata-Rata Running Time: {rata2Roulette.RunningTime} ms");

        Console.WriteLine("Tournament Selection :");
        Console.WriteLine($"\tRata-Rata Fitness Global Best : {rata2Tournament.Fitness:F8}");
        Console.WriteLine($"\tRata-Rata Generasi Global Best : {rata2Tournament.GenerasiGlobalBest}");
        Console.WriteLine($"\tRata-Rata Running Time: {rata2Tournament.RunningTime} ms");
    }

    private static T ChoicePrompt<T>(string prompt, List<(string label, T item)> choices, T defaultChoice, string defaultChoiceLabel)
    {
        for(int i = 0; i < choices.Count; i++)
            Console.WriteLine($"{i + 1}. {choices[i].label}");
        Console.Write($"{prompt}. [Default={defaultChoiceLabel}] : ");
        var pilih = Console.ReadLine();

        if (!string.IsNullOrEmpty(pilih) && int.TryParse(pilih, out var choice) && choice >= 1 && choice <= choices.Count)
            return choices[choice - 1].item;

        Console.WriteLine("Pilihan salah. Default digunakan!");
        return defaultChoice;    
    }
}