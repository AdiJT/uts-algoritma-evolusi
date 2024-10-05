using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core.Abstractions;

public interface ISeleksi
{
    List<Kromoson> Seleksi(AgenFungsiLinearDuaPeubah agen, List<Kromoson> populasi);
}
