using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class MaxOnesUtilities
    {
        static readonly Random Rand = new Random();
        static IInd[] _population;
        static double[] _fitnesses;
        static int _tournamentSize;
        private static int? _excludedIndex;

        public static IInd CreateIndividual()
        {
            string val = "";
            for (int i = 0; i < 20; i++)
            {
                val += Rand.Next(0, 2);
            }
            StringInd res = new StringInd(val);
            return res;
        }

        public static double ComputeFitness(IInd inputInd)
        {
            //Convert StringInd.Value to a string of bits, for each 1 in the bits array add 1 to the fitness,
            //divide the fitness by the amount of bits
            Char[] bits = inputInd.ToCharArray();
            var fitness = bits.Where(x => x == '1').Sum(x => 1.0);
            fitness = fitness/bits.Length;            
            return fitness;
        }

        public static Func<Tuple<IInd, IInd>> SelectTwoParents(IInd[] population, double[] fitnesses)
        {
            _population = population;
            _fitnesses = fitnesses;
            _tournamentSize = (int)Math.Round(_population.Length * 0.1);

            Func<Tuple<IInd, IInd>> res = TournamentSelection;
            return res;
        }

        private static Tuple<IInd, IInd> TournamentSelection()
        {
            IInd parent1 = RunTournament();
            IInd parent2 = RunTournament();
            _excludedIndex = null;

            return new Tuple<IInd, IInd>(parent1,parent2);
        }

        private static IInd RunTournament()
        {
            int fittestIndex = new int();
            for (int i = 0; i < _tournamentSize; i++)
            {
                int index = Rand.Next(0, _population.Length);
                if (_fitnesses[index] > _fitnesses[fittestIndex])
                {
                    if (index != _excludedIndex) 
                        fittestIndex = index;
                }
            }
            var res = _population[fittestIndex];
            _excludedIndex = fittestIndex;

            return res;
        } 

        public static Tuple<IInd, IInd> Crossover(Tuple<IInd, IInd> inp)
        {
            Char[] p1Bits = inp.Item1.ToCharArray();
            Char[] p2Bits = inp.Item2.ToCharArray();

            Char[] c1Bits = new Char[p1Bits.Length];
            Char[] c2Bits = new Char[p1Bits.Length];

            var splitpoint = Rand.Next(1, p1Bits.Length-1);

            p1Bits.Take(splitpoint).ToArray().CopyTo(c1Bits, 0);
            p2Bits.Skip(splitpoint).ToArray().CopyTo(c1Bits, splitpoint);

            p2Bits.Take(splitpoint).ToArray().CopyTo(c2Bits, 0);
            p1Bits.Skip(splitpoint).ToArray().CopyTo(c2Bits, splitpoint);

            IInd child1 = new StringInd(new string(c1Bits));
            IInd child2 = new StringInd(new string(c2Bits));
            return new Tuple<IInd, IInd>(child1, child2);
        }

        public static IInd Mutation(IInd inpStringInd, double rate)
        {
            Char[] inpBits = inpStringInd.ToCharArray();
            for (int i = 0; i < inpBits.Length; i++)
            {
                var r = Rand.NextDouble();
                if (r <= rate)
                {
                    switch (inpBits[i])
                    {
                        case '0':
                            inpBits[i] = '1';
                            break;
                        case '1':
                            inpBits[i] = '0';
                            break;
                    }
                }
            }
            IInd res = new StringInd(new string(inpBits));
            return res;
        }
    }
}
