using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface IEncoder<TAlel, TAsli>
{
    int PanjangGen { get; }
    List<TAlel> Encode(TAsli asli);
}
