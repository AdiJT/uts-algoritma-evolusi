namespace UTSAlgoEvolusi.Core.Abstractions;

public interface ICrossover
{
    (Kromoson anak1, Kromoson anak2) Crossover(AgenFungsiLinearDuaPeubah agen, Kromoson parent1, Kromoson parent2);
}
