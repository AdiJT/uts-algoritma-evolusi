using System;
using System.Collections.Generic;
using System.Linq;
using UTSAlgoEvolusi.Core.Utils;

namespace UTSAlgoEvolusi.Core
{
    public class Kromoson
    {
        public List<int> Gen { get; }

        private readonly (double bawah, double atas) _batasX;
        private readonly (double bawah, double atas) _batasY;
        private readonly int _presisi;
        
        public int JumlahGenX { get; }
        public int JumlahGenY { get; }

        public int JumlahGen => JumlahGenX + JumlahGenY;

        public int this[int index]
        {
            get => Gen[index];
            set => Gen[index] = value;
        }

        public Kromoson((double bawah, double atas) batasX, (double bawah, double atas) batasY, int presisi)
        {
            _batasX = batasX;
            _batasY = batasY;
            _presisi = presisi;
            
            JumlahGenX = (int)Math.Log2((_batasX.atas - _batasX.bawah) * Math.Pow(10, _presisi)) + 1;
            JumlahGenY = (int)Math.Log2((_batasY.atas - _batasY.bawah) * Math.Pow(10, _presisi)) + 1;

            Gen = new List<int>(Enumerable.Repeat(0, JumlahGen));
        }

        public Kromoson(Kromoson kromoson)
        {
            Gen = kromoson.Gen.Select(x => x).ToList();
            _batasX = kromoson._batasX;
            _batasY = kromoson._batasY;
            _presisi = kromoson._presisi;
            JumlahGenX = kromoson.JumlahGenX;
            JumlahGenY = kromoson.JumlahGenY;
        }

        public (double x, double y) Decoding()
        {
            var genX = BinaryConverter.ToInt(Gen.Take(JumlahGenX).ToList());
            var x = _batasX.bawah + genX * ((_batasX.atas - _batasX.bawah) / (Math.Pow(2, JumlahGenX) - 1));

            var genY = BinaryConverter.ToInt(Gen.Take(new Range(JumlahGenX, Index.End)).ToList());
            var y = _batasY.bawah + genY * ((_batasY.atas - _batasY.bawah) / (Math.Pow(2, JumlahGenY) - 1));

            return (x, y);
        }

        public static Kromoson Encoding(
            (double bawah, double atas) batasX, 
            (double bawah, double atas) batasY, 
            int presisi,
            double x,
            double y)
        {
            var kromoson = new Kromoson(batasX, batasY, presisi);

            var rasioX = (Math.Pow(2, kromoson.JumlahGenX) - 1) / (batasX.atas - batasX.bawah);
            var genX = (int)((x - batasX.bawah) * rasioX);

            var rasioY = (Math.Pow(2, kromoson.JumlahGenY) - 1) / (batasY.atas - batasY.bawah);
            var genY = (int)((y - batasY.bawah) * rasioY);

            var genXBinary = BinaryConverter.ToBinary(genX, kromoson.JumlahGenX);
            var genYBinary = BinaryConverter.ToBinary(genY, kromoson.JumlahGenY);

            for(int i = 0; i < kromoson.JumlahGenX; i++)
            {
                kromoson[i] = genXBinary[i];
            }

            for(int i = 0; i < kromoson.JumlahGenY; i++)
            {
                kromoson[kromoson.JumlahGenX + i] = genYBinary[i];
            }

            return kromoson;
        }
    }
}
