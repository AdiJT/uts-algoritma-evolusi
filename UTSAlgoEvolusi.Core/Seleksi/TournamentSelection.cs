using System;
using System.Collections.Generic;
using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Seleksi;

public class TournamentSelection<TAlel, TAsli> : ISeleksi<TAlel, TAsli>
{
    public List<Kromoson<TAlel>> Seleksi(
        List<Kromoson<TAlel>> populasi, 
        IEncoding<TAlel, TAsli> encoding, 
        Func<TAsli, double> fungsiObjektif, 
        JenisAgen jenis)
    {
        throw new NotImplementedException();
    }
}
