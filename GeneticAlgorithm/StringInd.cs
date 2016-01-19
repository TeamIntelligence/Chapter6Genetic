using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class StringInd : IInd
    {
        public readonly string Value;

        public StringInd(string v)
        {
            Value = v;
        }

        public double[] getDoubleArray()
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray()
        {
            Char[] res = Value.ToCharArray();
            return res;
        }

        public override string ToString()
        {
            return Value;
        }

        public int Length()
        {
            return Value.Length;
        }
    }
}
