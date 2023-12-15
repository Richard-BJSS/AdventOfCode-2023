namespace AdventOfCode._2023.Day15
{
    public sealed class LensLibrary(Utility.LinkedHashMap[] boxes)
    {
        public static int ComputeHash(string source) => source.Aggregate(0, (t, c) => ((t + c) * 17) % 256);

        public static LensLibrary Parse(string initialisationSequence)
        {
            var steps = initialisationSequence.Split(',');

            var boxes = Enumerable.Range(0, 256).Select(_ => new Utility.LinkedHashMap()).ToArray();

            for (var s = 0; s < steps.Length; s++)
            {
                var step = steps[s];

                var op = step.Split('=');

                var _ = op switch
                {

                    [var label, var focalLength] => boxes[ComputeHash(label)][label] = focalLength,

                    [var label] => boxes[ComputeHash(label[..^1])].Pop(label[..^1]),

                    _ => throw new ApplicationException("Invalid format for initialisation sequence.  Unable to Parse")
                };
            }

            return new(boxes);
        }

        public long ComputeFocusingPower()
        {
            var fp = 0L;

            for (var b = 0; b < boxes.Length; b++)
            {
                var box = boxes[b];

                if (0 >= box.Count) continue;

                for (var l = 0; l < box.Count; l++)
                {
                    var focalLength = int.Parse(box[l]);

                    fp += ((b + 1) * (l + 1)) * focalLength;
                }
            }

            return fp;
        }
    }
}
