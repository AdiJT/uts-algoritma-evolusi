using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface ISeleksi
{
    List<Kromoson> Seleksi(AgenBinary agen, List<Kromoson> populasi);
}
