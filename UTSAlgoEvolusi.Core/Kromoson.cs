using System;
using System.Collections.Generic;
using System.Linq;

namespace UTSAlgoEvolusi.Core;

public class Kromoson
{
    public List<int> Gen { get; }

    public int PanjangGen => Gen.Count;

    public int this[int index]
    {
        get => Gen[index];
        set 
        {
            if (value != 0 && value != 1)
                throw new ArgumentException("value harus 0 atau 1");

            Gen[index] = value;
        }
    }

    public Kromoson(int panjangGen)
    {
        Gen = Enumerable.Repeat(0, panjangGen).ToList();
    }

    public Kromoson(List<int> gen)
    {
        Gen = gen;
    }

    public Kromoson(Kromoson kromoson)
    {
        Gen = kromoson.Gen.Select(x => x).ToList();
    }
}
