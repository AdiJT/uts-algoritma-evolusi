using System;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Utils;
using System.Linq;
using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Encoding;

public class FungsiLinearDuaPeubahEncoding : IEncoding<int, LinearDuaPeubah>
{
    private (double bawah, double atas) _batasX;
    private (double bawah, double atas) _batasY;
    private int _presisi;

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

    public int Presisi
    {
        get => _presisi;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Presisi 0 atau negatif");

            _presisi = value;
        }
    }

    public int PanjangGenX => (int)Math.Log2((BatasX.atas - BatasX.bawah) * Math.Pow(10, Presisi)) + 1;
    public int PanjangGenY => (int)Math.Log2((BatasY.atas - BatasY.bawah) * Math.Pow(10, Presisi)) + 1;
    public int PanjangGen => PanjangGenX + PanjangGenY;

    public Kromoson<int> Encode(LinearDuaPeubah asli)
    {
        if (PanjangGenX <= 0)
            throw new Exception("PanjangGenX 0 atau negatif");

        if (PanjangGenY <= 0)
            throw new Exception("PanjangGenY 0 atau negatif");

        if (PanjangGen <= 0)
            throw new Exception("PanjangGen 0 atau negatif");

        asli.X = Math.Clamp(asli.X, BatasX.bawah, BatasX.atas);
        asli.Y = Math.Clamp(asli.Y, BatasY.bawah, BatasY.atas);

        var rasioX = (Math.Pow(2, PanjangGenX) - 1) / (BatasX.atas - BatasX.bawah);
        var xDecimal = (int)((asli.X - BatasX.bawah) * rasioX);

        var rasioY = (Math.Pow(2, PanjangGenY) - 1) / (BatasY.atas - BatasY.bawah);
        var yDecimal = (int)((asli.Y - BatasY.bawah) * rasioY);

        var xBiner = BinaryConverter.ToBinary(xDecimal, PanjangGenX);
        var yBiner = BinaryConverter.ToBinary(yDecimal, PanjangGenY);

        return new Kromoson<int>(xBiner.Concat(yBiner).ToList());
    }

    public LinearDuaPeubah Decode(Kromoson<int> kromoson)
    {
        var xDecimal = BinaryConverter.ToInt(kromoson.DaftarAlel.Take(PanjangGenX).ToList());
        var x = BatasX.bawah + xDecimal * ((BatasX.atas - BatasX.bawah) / (Math.Pow(2, PanjangGenX) - 1));

        var yDecimal = BinaryConverter.ToInt(kromoson.DaftarAlel.Take(new Range(PanjangGenY, Index.End)).ToList());
        var y = BatasY.bawah + yDecimal * ((BatasY.atas - BatasY.bawah) / (Math.Pow(2, PanjangGenY) - 1));

        return new LinearDuaPeubah { X = x, Y = y };
    }

    public List<Kromoson<int>> GeneratePopulasi(int jumlahPopulasi)
    {
        var populasi = new List<Kromoson<int>>();
        var random = new Random();

        for (int i = 0; i < jumlahPopulasi; i++)
        {
            var randX = random.NextDouble(BatasX.bawah, BatasX.atas);
            var randY = random.NextDouble(BatasY.bawah, BatasY.atas);

            populasi.Add(Encode(new LinearDuaPeubah { X=randX, Y=randY }));
        }

        return populasi;
    }

    public Kromoson<int> Mutasi(Kromoson<int> kromoson, int posisiAlel)
    {
        var newKromoson = new Kromoson<int>(kromoson);
        newKromoson.DaftarAlel[posisiAlel] = kromoson.DaftarAlel[posisiAlel] == 1 ? 0 : 1;

        return newKromoson;
    }
}
