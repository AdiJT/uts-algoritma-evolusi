using System;
using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface ISeleksi<TAlel, TAsli>
{
    List<Kromoson<TAlel>> Seleksi(List<Kromoson<TAlel>> populasi, IEncoding<TAlel, TAsli> encoding, Func<TAsli, double> fungsiObjektif, JenisAgen jenis);
}
