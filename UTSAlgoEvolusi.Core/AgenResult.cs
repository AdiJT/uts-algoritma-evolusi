using System.Collections.Generic;

namespace UTSAlgoEvolusi.Core;

public record AgenResult<TAlel>(
    Kromoson<TAlel> GlobalBest, 
    List<Kromoson<TAlel>> LocalBests, 
    double KonvergensiPopulasi, 
    int CounterGenerasi, 
    int GenerasiGlobalBest);
