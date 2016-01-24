using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class PregnancyAlgorithm<IInd> : IGeneticAlgorithm<IInd>
    {
        double crossoverRate;
        double mutationRate;
        bool elitism;
        int populationSize;
        int numIterations;
        Random r = new Random();

        public PregnancyAlgorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int numIterations)
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.elitism = elitism;
            this.populationSize = populationSize;
            this.numIterations = numIterations;
        }

        public IInd Run(Func<IInd> createIndividual, Func<IInd,double> computeFitness, Func<IInd[],double[],Func<Tuple<IInd,IInd>>> selectTwoParents, 
            Func<Tuple<IInd,IInd>,Tuple<IInd,IInd>> crossover, Func<IInd, double, IInd> mutation)
        {
            
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, populationSize).Select(i => createIndividual()).ToArray();

            var currentPopulation = initialPopulation;
            
            for (int generation = 0; generation < numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();

                var nextPopulation = new IInd[populationSize]; 

                // apply elitism
                int startIndex; 
                if(elitism)
                {
                    startIndex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<IInd,double>(individual,fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderByDescending(tuple => tuple.Item2); // item2 is the fitness
                    var bestIndividual = populationSorted.First();
                    nextPopulation[0] = bestIndividual.Item1;
                } else
                {
                    startIndex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);
                
                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < populationSize; newInd++)
                {
                    // select two parents
                    var parents = getTwoParents();
                    
                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<IInd,IInd> offspring;
                    if (r.NextDouble() < crossoverRate)
                        offspring = crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = mutation(offspring.Item1, mutationRate);
                    if (newInd < populationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = mutation(offspring.Item2, mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;
                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }


            // recompute the fitnesses on the final population and return the best individual
            var finalFitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();
            var sse = finalFitnesses.OrderBy(c => c).First();

            PregnancyUtilities.sse = sse;
            IInd mC = currentPopulation.Select((individual, index) => new Tuple<IInd, double>(individual, finalFitnesses[index])).OrderBy(tuple => tuple.Item2).First().Item1;
            PregnancyUtilities.modelCoefficients = mC;

            Console.WriteLine("Best fitness: {0}", sse);

            return mC;
        }

    }
}
