using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface IEncoding<TAlel, TAsli>
{
    Kromoson<TAlel> Encode(TAsli asli);
    TAsli Decode(Kromoson<TAlel> kromoson);
    Kromoson<TAlel> Mutasi(Kromoson<TAlel> kromoson, int indexAlel);
    List<Kromoson<TAlel>> GeneratePopulasi(int jumlahPopulasi);
}
