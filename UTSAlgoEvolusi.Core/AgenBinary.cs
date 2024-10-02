using System;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core
{
    public class AgenBinary
    {
        public int JumlahGenerasi { get; set; } = 1000;
        public int JumlahPopulasi { get; set; } = 10;
        public double ProbabilitasCrossover { get; set; } = 0.50;
        public double ProbabilitasMutasi { get; set; } = 0.1;
        public double BatasKonvergensiPopulasi { get; set; } = 0.8;

        private readonly (double bawah, double atas) _batasXFungsiObjektif;
        private readonly (double bawah, double atas) _batasYFungsiObjektif;
        private readonly int _batasPresisi;
        private readonly Func<double, double, double> _fungsiObjektif;

        private readonly ISeleksi _seleksi;
        private readonly ICrossover _crossover;

        public AgenBinary(
            Func<double, double, double> fungsiObjektif,
            (double bawah, double atas) batasXFungsiObjektif,
            (double bawah, double atas) batasYFungsiObjektif,
            int batasPresisi,
            ISeleksi seleksi,
            ICrossover crossover)
        {
            if (batasXFungsiObjektif.bawah > batasXFungsiObjektif.atas)
                throw new ArgumentException("Batas X Bawah melebihi Batas X Atas");

            if (batasYFungsiObjektif.bawah > batasYFungsiObjektif.atas)
                throw new ArgumentException("Batas Y Bawah melebihi Batas Y Atas");

            if (batasPresisi <= 0)
                throw new ArgumentOutOfRangeException(nameof(batasPresisi), "0 atau negatif");

            _fungsiObjektif = fungsiObjektif;
            _batasXFungsiObjektif = batasXFungsiObjektif;
            _batasYFungsiObjektif = batasYFungsiObjektif;
            _batasPresisi = batasPresisi;
            _seleksi = seleksi;
            _crossover = crossover;
        }
    }
}
