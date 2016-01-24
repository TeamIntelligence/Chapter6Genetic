using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class PregnancyUtilities
    {
        static readonly Random Rand = new Random();
        private static List<int[]> _dataSet = new List<int[]>();
        private static List<double[]> _testSet = new List<double[]>();
        const double minVal = -1;
        const double maxVal = 1;
        public static double sse {get; set;}
        //public static IInd modelCoefficients { get; set; } 
        static IInd[] _population;
        static double[] _fitnesses;
        static int _tournamentSize;

        static int? _excludedIndex;
        internal static object modelCoefficients { get; set; }

        public static DoubleInd CreateIndividual()
        {
            if (!_dataSet.Any())
                InitDataSet();

            double[] val = new double[_dataSet[1].Length-1];
            for (int i = 0; i < val.Length; i++)
            {
                val[i] = Rand.NextDouble() * (maxVal - minVal) + minVal;
            }
            
            return new DoubleInd(val);
        }

        public static double ComputeFitness(IInd inputInd)
        {
            double[] squaredErrors = new double[_dataSet.Count];

            for (int i = 0; i < _dataSet.Count; i++)
            {
                double sP = 0;
                var line = _dataSet[i];
                for (int j = 0; j < line.Length-1; j++)
                {
                    var dataValue = line[j];
                    var indValue = inputInd.getDoubleArray()[j];
                    sP += (dataValue*indValue);
                }
                squaredErrors[i] = Math.Pow((line[line.Length - 1] - sP), 2);
            }

            return squaredErrors.Sum();
        }

        private static void InitDataSet()
        {
            string dir = Directory.GetCurrentDirectory() + "\\..\\..\\datasetfig6-7.csv";
            var reader = new StreamReader(File.OpenRead(dir));
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');

                int[] parsedValues = new int[values.Length + 1];

                for (int i = 0; i < values.Length; i++)
                {
                    if (i == values.Length - 1)
                    {
                        parsedValues[i] = 1;
                    }

                    string value = values[i];
                    int parsedValue;
                    bool isNumeric = int.TryParse(value, out parsedValue);

                    if (isNumeric)
                    {
                        if(i == values.Length - 1)
                        {
                            parsedValues[i + 1] = parsedValue;

                        }
                        else
                        {
                            parsedValues[i] = parsedValue;
                        }
                    }
                }

                bool allValuesNull = parsedValues.All(parsedValue => parsedValue == 0);

                if(!allValuesNull)
                    _dataSet.Add(parsedValues);
            }
            initTestSet();
        }

        private static void initTestSet()
        {
            string dir = Directory.GetCurrentDirectory() + "\\..\\..\\testSet.csv";
            var reader = new StreamReader(File.OpenRead(dir));
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                double[] parsedValues = new double[values.Length + 1];

                for (int i = 0; i < values.Length; i++)
                {
                    if (i == values.Length - 1)
                    {
                        parsedValues[i] = 1;
                    }

                    string value = values[i];
                    int parsedValue;
                    bool isNumeric = int.TryParse(value, out parsedValue);

                    if (isNumeric)
                    {
                        if (i == values.Length - 1)
                        {
                            parsedValues[i + 1] = parsedValue;

                        }
                        else
                        {
                            parsedValues[i] = parsedValue;
                        }
                    }
                }

                bool allValuesNull = parsedValues.All(parsedValue => parsedValue == 0);

                if (!allValuesNull)
                    _testSet.Add(parsedValues);
            }
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

            return new Tuple<IInd, IInd>(parent1, parent2);
        }

        private static IInd RunTournament()
        {
            int fittestIndex = new int();
            for (int i = 0; i < _tournamentSize; i++)
            {
                int index = Rand.Next(0, _population.Length);
                if (_fitnesses[index] < _fitnesses[fittestIndex])
                {
                    if (fittestIndex != _excludedIndex)
                        fittestIndex = index;
                }
            }
            var res = _population[fittestIndex];
            _excludedIndex = fittestIndex;
            return res;
        }

        public static Tuple<IInd, IInd> Crossover(Tuple<IInd, IInd> inp)
        {
            //Two point crossover
            var p1Bits = inp.Item1.getDoubleArray();
            var p2Bits = inp.Item2.getDoubleArray();

            var c1Bits = new double[p1Bits.Length];
            p1Bits.CopyTo(c1Bits, 0);
            var c2Bits = new double[p1Bits.Length];
            p2Bits.CopyTo(c2Bits, 0);

            var splitpoint1 = Rand.Next(1, p1Bits.Length - 1);
            var splitpoint2 = Rand.Next(splitpoint1 + 1, p1Bits.Length);

            for (int i = splitpoint1; i <= splitpoint2; i++)
            {
                c1Bits[i] = p2Bits[i];
                c2Bits[i] = p1Bits[i];
            }

            IInd child1 = new DoubleInd(c1Bits);
            IInd child2 = new DoubleInd(c2Bits);

            return new Tuple<IInd, IInd>(child1, child2);
        }

        public static IInd Mutation(IInd inpStringInd, double rate)
        {
            double[] values = inpStringInd.getDoubleArray();
            for (int i = 0; i < values.Length; i++)
            {
                var r = Rand.NextDouble();
                if (r <= rate)
                {
                    values[i] = Rand.NextDouble() * (maxVal - minVal) + minVal;
                }
            }
            IInd res = new DoubleInd(values);
            return res;

        }

        public static double ComputeTSS()
        {
            double sum      = 0.0;

            foreach(var row in _dataSet)
            {
                sum += row.Last();
            }

            double se = (sum / _dataSet.Count) * (sum / _dataSet.Count);

            return se * _dataSet.Count;
        }

        public static List<double> PredictTestSet()
        {
            List<double> res = new List<double>();
            foreach(var row in _testSet)
            {
                double predictedRow = 0;
                for (var i = 0; i < row.Length - 1; i++)
                {
                    // sumprod += row i * coeff i 
                    predictedRow += row[i] * ((DoubleInd)modelCoefficients).Value[i];

                }
                predictedRow += row[row.Length - 1];
                res.Add(predictedRow);
            }

            return res;
        }

        public static void AddPredictions()
        {
            List<double> predictions = PredictTestSet();
            for (int j = 0; j < _testSet.Count; j++)
            {
                var row = _testSet.ElementAt(j);
                var rowCopy = new double[row.Length + 1];
                for(int i = 0; i < row.Length; i++)
                {
                    rowCopy[i] = row[i];
                }
                rowCopy[rowCopy.Length-1] = predictions.ElementAt(j);

                _testSet.RemoveAt(j);
                _testSet.Insert(j, rowCopy);
                //var replaceRow = _testSet.ElementAt(j);
                //replaceRow = rowCopy;
                //_testSet.  (j, rowCopy);
            }
            createCSV(_testSet);
        }

        public static void createCSV(List<double[]> data)
        {
            using (var file = File.CreateText(Directory.GetCurrentDirectory() + "\\..\\..\\testPredictions.csv"))
            {
                foreach (var arr in data)
                {
                    file.WriteLine(string.Join(",", arr));
                }
            }
        }
    }
}
