using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Encoding;
using UTSAlgoEvolusi.Core.Seleksi;

var fungsiObjektif = (LinearDuaPeubah args) => 100 * (args.X * args.X - args.Y) * (args.X * args.X - args.Y) + (1 - args.X) * (1 - args.X);

var encoding = new FungsiLinearDuaPeubahEncoding() 
{
    Presisi = 4,
    BatasX = (-2.048, 2.048),
    BatasY = (-2.048, 2.048)
};

var crossover = new TwoPointCrossover<int>();
var roulleteWheel = new TournamentSelection<int, LinearDuaPeubah>();

var agen = new Agen<int, LinearDuaPeubah>(fungsiObjektif, roulleteWheel, encoding, crossover)
{
    JenisAgen = JenisAgen.Min,
    JumlahGenerasi = 100,
    JumlahPopulasi = 300,
    BatasKonvergensiPopulasi = 0.8,
    ProbabilitasMutasi = 0.02,
    ProbabilitasCrossover = 0.7
};

Console.WriteLine("Meminimumkan Fungsi Objektif : f(x, y) = 100(x^2 - y)^2 + (1 - x)^2");
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

var jumlahTes = 100;
Console.WriteLine($"Tes Roulette Vs Tournament Selection. Jumlah Tes : {jumlahTes}");

var daftarHasilTesRoulette = new List<double>();
var daftarHasilTesTournament = new List<double>();

agen.Seleksi = new RouletteWheel<int, LinearDuaPeubah>();
for (int i = 0; i < jumlahTes; i++)
{
    var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
    var gb = encoding.Decode(r.GlobalBest);
    daftarHasilTesRoulette.Add(fungsiObjektif(gb));
    Console.Write("|");
}

agen.Seleksi = new TournamentSelection<int, LinearDuaPeubah>();
for (int i = 0; i < jumlahTes; i++)
{
    var r = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
    var gb = encoding.Decode(r.GlobalBest);
    daftarHasilTesTournament.Add(fungsiObjektif(gb));
    Console.Write("|");
}

var rata2Roulette = daftarHasilTesRoulette.Average();
var rata2Tournament = daftarHasilTesTournament.Average();

Console.WriteLine($"\nHasil Roullette Vs Tournament Selection");
Console.WriteLine($"Rata-rata fitness global best Roullette Wheel : {rata2Roulette:F8}");
Console.WriteLine($"Rata-rata fitness global best Tournament Selection : {rata2Tournament:F8}");