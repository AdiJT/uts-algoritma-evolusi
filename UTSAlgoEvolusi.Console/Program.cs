using System.Diagnostics;
using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Encoding;
using UTSAlgoEvolusi.Core.Seleksi;

internal class Program
{
    private record TestResult(double FitnessGlobalBest, int GenerasiGlobalBest, double Runtime);

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

        ICrossover<int> crossover = new SinglePointCrossover<int>();
        ISeleksi<int, LinearDuaPeubah> seleksi = new RouletteWheel<int, LinearDuaPeubah>();

        Console.WriteLine("\n1. Roullette Wheel");
        Console.WriteLine("2. Tournament Selection");
        Console.Write("Pilih metode seleksi : ");
        var pilih = Console.ReadLine();

        if (!string.IsNullOrEmpty(pilih))
            seleksi = pilih switch
            {
                "1" => new RouletteWheel<int, LinearDuaPeubah>(),
                "2" => new TournamentSelection<int, LinearDuaPeubah>(),
                _ => new TournamentSelection<int, LinearDuaPeubah>(),
            };
        else
            Console.WriteLine("Pilihan Salah! Metode seleksi [1] digunakan!");

        Console.WriteLine("\n1. Single Point Crossover (SPX)");
        Console.WriteLine("2. Two Point Crossover (TPX)");
        Console.Write("Pilih metode kawin silang : ");
        pilih = Console.ReadLine();

        if (!string.IsNullOrEmpty(pilih))
            crossover = pilih switch
            {
                "1" => new SinglePointCrossover<int>(),
                "2" => new TwoPointCrossover<int>(),
                _ => new SinglePointCrossover<int>(),
            };
        else
            Console.WriteLine("Pilihan Salah! Metode kawin silang [1] digunakan!");



        var agen = new Agen<int, LinearDuaPeubah>(fungsiObjektif, seleksi, encoding, crossover)
        {
            JenisAgen = JenisAgen.Min,
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

        var daftarHasilTesRoulette = new List<TestResult>();
        var daftarHasilTesTournament = new List<TestResult>();
        var stopwatch = new Stopwatch();

        agen.Seleksi = new RouletteWheel<int, LinearDuaPeubah>();
        for (int i = 0; i < jumlahTes; i++)
        {
            stopwatch.Start();
            var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
            stopwatch.Stop();

            var gb = encoding.Decode(r.GlobalBest);
            daftarHasilTesRoulette.Add(
                new TestResult(
                    fungsiObjektif(gb),
                    r.GenerasiGlobalBest,
                    stopwatch.Elapsed.TotalMilliseconds));
            stopwatch.Restart();

            Console.Write("|");
        }

        agen.Seleksi = new TournamentSelection<int, LinearDuaPeubah>();
        for (int i = 0; i < jumlahTes; i++)
        {
            stopwatch.Start();
            var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
            stopwatch.Stop();

            var gb = encoding.Decode(r.GlobalBest);
            daftarHasilTesTournament.Add(
                new TestResult(
                    fungsiObjektif(gb),
                    r.GenerasiGlobalBest,
                    stopwatch.Elapsed.TotalSeconds));
            stopwatch.Restart();

            Console.Write("|");
        }

        var rata2Roulette = new
        {
            Fitness = daftarHasilTesRoulette.Average(r => r.FitnessGlobalBest),
            GenerasiGlobalBest = daftarHasilTesRoulette.Average(r => r.GenerasiGlobalBest),
            RunningTime = daftarHasilTesRoulette.Average(r => r.Runtime)
        };

        var rata2Tournament = new
        {
            Fitness = daftarHasilTesTournament.Average(r => r.FitnessGlobalBest),
            GenerasiGlobalBest = daftarHasilTesTournament.Average(r => r.GenerasiGlobalBest),
            RunningTime = daftarHasilTesTournament.Average(r => r.Runtime)
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
}