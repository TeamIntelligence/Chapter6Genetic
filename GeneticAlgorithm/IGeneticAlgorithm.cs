using System;

namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithm<IInd>
    {
        IInd Run(Func<IInd> createIndividual, Func<IInd,double> computeFitness, Func<IInd[],double[],Func<Tuple<IInd,IInd>>> selectTwoParents, 
            Func<Tuple<IInd,IInd>,Tuple<IInd,IInd>> crossover, Func<IInd, double, IInd> mutation);
    }
}