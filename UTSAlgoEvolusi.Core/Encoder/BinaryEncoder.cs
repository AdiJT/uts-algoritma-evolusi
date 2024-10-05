﻿using System;
using System.Collections.Generic;
using UTSAlgoEvolusi.Core.Abstractions;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Core.Encoder;

public class BinaryEncoder : IEncoder<int, LinearDuaPeubah>
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

    public List<int> Encode(LinearDuaPeubah asli)
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
        var xDecimal = (int)((asli.X - _batasX.bawah) * rasioX);

        var rasioY = (Math.Pow(2, PanjangGenY) - 1) / (BatasY.atas - BatasY.bawah);
        var yDecimal = (int)((asli.Y - BatasY.bawah) * rasioY);

        var xBiner = BinaryConverter.ToBinary(xDecimal, PanjangGenX);
        var yBiner = BinaryConverter.ToBinary(yDecimal, PanjangGenY);

        return [..xBiner, ..yBiner];
    }
}
