namespace AdventOfCode
{
    public static partial class Numerics
    {
        /// <summary>
        /// Also known as the Least Common Multiple
        /// </summary>
        /// <param name="xs">The set of numbers we are calculating the LCM for</param>
        /// <returns>The lowest common multiple shared by all numbers in the set</returns>
        public static long LowestCommonMultiple(this IEnumerable<long> xs) => xs.Aggregate((a, b) => Math.Abs(a * b) / GreatestCommonDivisor(a, b));

        private static long GreatestCommonDivisor(long a, long b) => 0 == b ? a : GreatestCommonDivisor(b, a % b);
    }
}
