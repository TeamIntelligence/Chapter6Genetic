using System.Runtime.InteropServices;

namespace GeneticAlgorithm
{
    public interface IInd
    {


        double[] getDoubleArray();

        char[] ToCharArray();

        string ToString();

        int Length();
    }
}