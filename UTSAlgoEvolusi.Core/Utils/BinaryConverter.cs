using System;
using System.Collections.Generic;
using System.Linq;

namespace UTSAlgoEvolusi.Core.Utils
{
    public static class BinaryConverter
    {
        public static List<int> ToBinary(int angka, int maksJumlahDigit)
        {
            if (angka < 0)
                throw new ArgumentOutOfRangeException(nameof(angka), "Tidak dapat konversi bilangan negatif");

            if (angka == 0)
                return new List<int>(Enumerable.Range(0, maksJumlahDigit).Select(x => 0));

            var jumlahDigit = (int)Math.Log(angka, 2);


            if (jumlahDigit > maksJumlahDigit)
                throw new ArgumentOutOfRangeException(
                    nameof(maksJumlahDigit),
                    $"{nameof(maksJumlahDigit)}:{maksJumlahDigit} Tidak cukup untuk representasi biner " +
                    $"{nameof(angka)}:{angka}. Jumlah yang dibutuhkan:{jumlahDigit}");

            var biner = new List<int>();

            while (angka > 0)
            {
                biner.Add(angka % 2);
                angka = angka >> 1;
            }

            biner.Add(angka);

            return biner.PadLeft(maksJumlahDigit, 0).ToList();
        }

        public static int ToInt(List<int> biner)
        {
            var angka = 0;

            for(int i = 0; i < biner.Count; i++)
            {
                angka += biner[i] * (int)Math.Pow(2, biner.Count - i - 1);
            }

            return angka;
        }
    }
}
