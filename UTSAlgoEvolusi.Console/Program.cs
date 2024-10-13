using UTSAlgoEvolusi.Core;
using UTSAlgoEvolusi.Core.Crossover;
using UTSAlgoEvolusi.Core.Encoding;
using UTSAlgoEvolusi.Core.Seleksi;

var fungsiObjektif = (LinearDuaPeubah args) => 
    (args.X * args.X - 10 * Math.Cos(2 * Math.PI * args.X)) + 
    (args.Y * args.Y - 10 * Math.Cos(2 * Math.PI * args.Y)) + 20;

var encoding = new FungsiLinearDuaPeubahEncoding() 
{
    Presisi = 4,
    BatasX = (-5.12, 5.12),
    BatasY = (-5.12, 5.12)
};

var spx = new SinglePointCrossover<int>();
var roulleteWheel = new RouletteWheel<int, LinearDuaPeubah>();

var agen = new Agen<int, LinearDuaPeubah>(fungsiObjektif, roulleteWheel, encoding, spx)
{
    JenisAgen = JenisAgen.Min,
    JumlahGenerasi = 2000,
    JumlahPopulasi = 10,
    BatasKonvergensiPopulasi = .8,
};

Console.WriteLine("Meminimumkan Fungsi Objektif : f(x, y) = (x^2 - 10cos(2PIx)) + y^2 - 10cos(2PIy) + 20");
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
    Console.WriteLine($"Percobaa Ke-{i+1}. Global Best : (x : {globalBest.X:F8}, y : {globalBest.Y:F8}), f(x, y) = {fungsiObjektif(globalBest):F8}");
}