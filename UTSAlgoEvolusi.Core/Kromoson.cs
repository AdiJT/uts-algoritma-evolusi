using System;
using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core
{
    public class Kromoson
    {
        public List<int> X { get; set; } = [];
        public List<int> Y { get; set; } = [];

        private readonly (double bawah, double atas) _batasX;
        private readonly (double bawah, double atas) _batasY;
        private readonly int _batasPresisi;
        
        private readonly int _jumlahGenX;
        private readonly int _jumlahGenY;

        public Kromoson((double bawah, double atas) batasX, (double bawah, double atas) batasY, int batasPresisi)
        {
            _batasX = batasX;
            _batasY = batasY;
            _batasPresisi = batasPresisi;
            
            _jumlahGenX = (int)(Math.Log(_batasX.atas - _batasX.bawah, 2) * Math.Pow(10, _batasPresisi));
            _jumlahGenY = (int)(Math.Log(_batasY.atas - _batasY.bawah, 2) * Math.Pow(10, _batasPresisi));
            X = new List<int>(_jumlahGenX);
            Y = new List<int>(_jumlahGenY);
        }

        //public (double x, double y) Decoding()
        //{

        //}
    }
}
