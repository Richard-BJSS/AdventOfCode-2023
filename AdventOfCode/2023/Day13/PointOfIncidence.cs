namespace AdventOfCode._2023.Day13
{
    public static class PointOfIncidence
    {
        public static int[] ReflectionScores(string[] orig)
        {
            var strings = orig.ToArray();
            var scores = new int[2];

            foreach (var reversed in new[] { false, true })
            {
                for (var j = 1; j < strings.Length; j += 2)
                {
                    var smudges = 1 - StringExtensions.HammingDistance(strings[0], strings[j]);

                    if (smudges < 0) continue;

                    var radius = (j + 1) / 2;
                    var reflection = true;

                    for (var k = 1; k < radius; k++)
                    {
                        smudges -= StringExtensions.HammingDistance(strings[k], strings[j - k]);

                        if (smudges < 0)
                        {
                            reflection = false;
                            break;
                        }
                    }

                    if (reflection)
                    {
                        scores[(smudges + 1) % 2] += reversed ? strings.Length - radius : radius;
                    }
                }

                if (reversed) return scores;

                strings = strings.Reverse().ToArray();
            }

            throw new Exception();
        }
    }
}
