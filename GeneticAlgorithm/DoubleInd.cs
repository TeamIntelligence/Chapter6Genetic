using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class DoubleInd : IInd
    {
        public readonly double[] Value;

        public DoubleInd(double[] value)
        {
            Value = value;
        }

        public double[] getDoubleArray()
        {
            return Value;
        }

        public char[] ToCharArray()
        {
            Char[] res = Value.ToString().ToCharArray();
            return res;
        }

        public override string ToString()
        {
            string res = "";
            foreach (var x in Value)
            {
                res += x.ToString() + "\n ";
            }
            return res;
        }

        public int Length()
        {
            return Value.ToString().Length;
        }
    }
}
