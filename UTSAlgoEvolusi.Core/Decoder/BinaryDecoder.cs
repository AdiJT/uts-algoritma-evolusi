using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Core.Decoder;

public class BinaryDecoder : IDecoder<int, LinearDuaPeubah>
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

    public LinearDuaPeubah Decode(List<int> encoded)
    {
        var xDecimal = BinaryConverter.ToInt(encoded.Take(PanjangGenX).ToList());
        var x = BatasX.bawah + xDecimal * ((BatasX.atas - BatasX.bawah) / (Math.Pow(2, PanjangGenX) - 1));

        var yDecimal = BinaryConverter.ToInt(encoded.Take(new Range(PanjangGenY, Index.End)).ToList());
        var y = BatasY.bawah + yDecimal * ((BatasY.atas - BatasY.bawah) / (Math.Pow(2, PanjangGenY) - 1));

        return new LinearDuaPeubah { X = x, Y = y };
    }
}
