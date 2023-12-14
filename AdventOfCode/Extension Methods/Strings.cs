namespace AdventOfCode
{
    public static partial class StringExtensions
    {
        public static int HammingDistance(string s1, string s2) => s1.Zip(s2).Where(p => p.First != p.Second).Count();

    }
}
