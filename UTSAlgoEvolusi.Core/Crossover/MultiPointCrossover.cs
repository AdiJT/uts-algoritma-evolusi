using UTSAlgoEvolusi.Core.Abstractions;

namespace UTSAlgoEvolusi.Core.Crossover;

public class MultiPointCrossover<TAlel> : ICrossover<TAlel>
{
    public (Kromoson<TAlel> anak1, Kromoson<TAlel> anak2) Crossover(Kromoson<TAlel> parent1, Kromoson<TAlel> parent2)
    {
        throw new System.NotImplementedException();
    }
}
