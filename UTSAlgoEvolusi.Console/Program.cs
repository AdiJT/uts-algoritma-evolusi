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
var roulleteWheel = new RouletteWheel<int, LinearDuaPeubah>();

var agen = new Agen<int, LinearDuaPeubah>(fungsiObjektif, roulleteWheel, encoding, crossover)
{
    JenisAgen = JenisAgen.Min,
    JumlahGenerasi = 200,
    JumlahPopulasi = 250,
    BatasKonvergensiPopulasi = .8,
};

Console.WriteLine("Meminimumkan Fungsi Objektif : f(x, y) = 100(x^2 - y)^2 + (1 - x)^2");
Console.WriteLine($"Batas: {encoding.BatasX.bawah} <= x <= {encoding.BatasX.atas} ");
Console.WriteLine($"Batas: {encoding.BatasY.bawah} <= y <= {encoding.BatasY.atas} ");
Console.WriteLine($"Jumlah Generasi : {agen.JumlahGenerasi}");
Console.WriteLine($"Jumlah Populasi : {agen.JumlahPopulasi}");
Console.WriteLine($"Probabilitas Crossover : {agen.ProbabilitasCrossover:P2}");
Console.WriteLine($"Probabilitas Mutasi : {agen.ProbabilitasMutasi:P2}");
Console.WriteLine($"Batas Konvergensi : {agen.BatasKonvergensiPopulasi:P2}");

var result = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));

var globalBest = encoding.Decode(result.GlobalBest);
Console.WriteLine($"Global Best : (x : {globalBest.X:F8}, y : {globalBest.Y:F8}), f(x, y) = {fungsiObjektif(globalBest):F8}");
Console.WriteLine($"Generasi Global Best : {result.GenerasiGlobalBest}");
Console.WriteLine("Local Best");

for (int i = 0; i < result.LocalBests.Count; i++)
{
    var kromoson = result.LocalBests[i];
    var decoded = encoding.Decode(kromoson);
    Console.WriteLine($"Generasi {i + 1} : (x : {decoded.X:F8}, y : {decoded.Y:F8}), f(x, y) = {fungsiObjektif(decoded):F8}");
}

var jumlahPercobaan = 100;
for (int i = 0; i < jumlahPercobaan; i++)
{
    result = agen.Execute(encoding.GeneratePopulasi(agen.JumlahPopulasi));
    globalBest = encoding.Decode(result.GlobalBest);
    Console.WriteLine($"Percobaan Ke-{i+1}. Global Best : (x : {globalBest.X:F8}, y : {globalBest.Y:F8}), f(x, y) = {fungsiObjektif(globalBest):F8}");
}