using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Program
    {
        static Func<IInd> createIndividual;
        static Func<IInd, double> computeFitness;
        static Func<IInd[], double[], Func<Tuple<IInd, IInd>>> selectTwoParents;
        static Func<Tuple<IInd, IInd>, Tuple<IInd, IInd>> crossover;
        static Func<IInd, double, IInd> mutation;

        static void Main(string[] args)
        { 
            /*
            Func<StringInd> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<StringInd,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<StringInd[],double[],Func<Tuple<StringInd,StringInd>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<StringInd, StringInd>, Tuple<StringInd, StringInd>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<StringInd, double, StringInd> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */

            loadMaxOnesProblem();
            GeneticAlgorithm<IInd> maxOnesProblem = new GeneticAlgorithm<IInd>(0.9, 0.01, true, 30, 10);
            var mOSolution = maxOnesProblem.Run(
                createIndividual,
                computeFitness,
                selectTwoParents,
                crossover,
                mutation);

            Console.WriteLine("Solution MaxOne Problem: ");
            Console.WriteLine(mOSolution);

            loadPregnancyProblem();
            IGeneticAlgorithm<IInd> PregnancyProblem = new PregnancyAlgorithm<IInd>(0.75, 0.05, true, 30, 200); 
            var pSolution = PregnancyProblem.Run(
                createIndividual,
                computeFitness,
                selectTwoParents,
                crossover,
                mutation);

            var tss = PregnancyUtilities.ComputeTSS();
            var sse = PregnancyUtilities.sse;
            var explSos = tss - sse;
            var rSquared = explSos / tss;

            PregnancyUtilities.AddPredictions();

            Console.WriteLine("Total Sum of Squares = " + tss);
            Console.WriteLine("Sum of Squared Error = " + sse);
            Console.WriteLine("Explained Sum of Squares = " + explSos);
            Console.WriteLine("R squared = " + rSquared);

            Console.WriteLine("Solution Pregnancy Problem: ");

            foreach (var x in pSolution.getDoubleArray())
            {
                Console.WriteLine(x);
            }
             
            Console.ReadLine();
        }

        private static void loadMaxOnesProblem()
        {
            createIndividual = MaxOnesUtilities.CreateIndividual;
            computeFitness = MaxOnesUtilities.ComputeFitness;
            selectTwoParents = MaxOnesUtilities.SelectTwoParents;
            crossover = MaxOnesUtilities.Crossover;
            mutation = MaxOnesUtilities.Mutation;
        }

        private static void loadPregnancyProblem()
        {
            createIndividual = PregnancyUtilities.CreateIndividual;
            computeFitness = PregnancyUtilities.ComputeFitness;
            selectTwoParents = PregnancyUtilities.SelectTwoParents;
            crossover = PregnancyUtilities.Crossover;
            mutation = PregnancyUtilities.Mutation;
        }


    }
}
