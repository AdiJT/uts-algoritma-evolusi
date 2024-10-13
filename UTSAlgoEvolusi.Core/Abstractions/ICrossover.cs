namespace UTSAlgoEvolusi.Core.Abstractions;

public interface ICrossover<TAlel>
{
    (Kromoson<TAlel> anak1, Kromoson<TAlel> anak2) Crossover(Kromoson<TAlel> parent1, Kromoson<TAlel> parent2);
}
