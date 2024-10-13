using System;
using System.Collections.Generic;
using System.Linq;

namespace UTSAlgoEvolusi.Core;

public class Kromoson<TAlel>
{
    public List<TAlel> DaftarAlel { get; }

    public Kromoson(int jumlahAlel)
    {
        DaftarAlel = new List<TAlel>(new TAlel[jumlahAlel]);
    }

    public Kromoson(List<TAlel> gen)
    {
        DaftarAlel = gen.Select(x => x).ToList();
    }

    public Kromoson(Kromoson<TAlel> kromoson)
    {
        DaftarAlel = kromoson.DaftarAlel.Select(x => x).ToList();
    }
}
