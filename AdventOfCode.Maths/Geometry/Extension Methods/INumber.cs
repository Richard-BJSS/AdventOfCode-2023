using System.Numerics;

namespace AdventOfCode.Maths
{
    public static class NumberExtensions
    {
        public static T Modulo<T>(this T a, T modulus)
            where T : INumber<T>
        {
            return (a % modulus + modulus) % modulus;
        }
    }
}
