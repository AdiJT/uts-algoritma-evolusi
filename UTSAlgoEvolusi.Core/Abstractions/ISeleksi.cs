using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions
{
    public interface ISeleksi
    {
        List<Kromoson> Seleksi(List<Kromoson> populasi);
    }
}
