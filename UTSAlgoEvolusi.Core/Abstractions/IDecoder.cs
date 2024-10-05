using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface IDecoder<TAlel, TAsli>
{
    int PanjangGen { get; }
    TAsli Decode(List<TAlel> encoded);
}
