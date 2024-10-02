namespace UTSAlgoEvolusi.Core.Abstractions
{
    public interface ICrossover
    {
        (Kromoson anak1, Kromoson anak2) Crossover(Kromoson parent1, Kromoson parent2, double probabilitasCrossover);
    }
}
