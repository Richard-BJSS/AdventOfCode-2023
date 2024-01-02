namespace AdventOfCode._2023.Day25
{
    public class WiringDiagram(string[] components, (string, string)[] componentConnections)
    {
        public static WiringDiagram Parse(string[] rawWiringDiagram)
        {
            var components = new List<string>();

            var componentConnections = new List<(string, string)>();

            foreach (var rawComponent in rawWiringDiagram)
            {
                var xs = rawComponent.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                var component = xs[0];

                xs = xs[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var connectedComponents = new HashSet<string>(xs);

                if (!components.Contains(component)) components.Add(component);

                foreach (var connectedComponent in connectedComponents)
                {
                    if (!components.Contains(connectedComponent)) components.Add(connectedComponent);

                    if (!componentConnections.Contains((component, connectedComponent)) && !componentConnections.Contains((connectedComponent, component))) componentConnections.Add((component, connectedComponent));
                }
            }

            return new(components.ToArray(), componentConnections.ToArray());
        }

        public List<List<string>> FindMinimumCut()
        {
            // Using Karger's Algorithm to find the minimum cut of a graph, as defined in https://web.stanford.edu/class/archive/cs/cs161/cs161.1172/CS161Lecture16.pdf
            // This is a Monte Carlo algo so isn't guaranteed to be correct, but does have a high probability of being so

            List<List<string>> xs;

            var rnd = new Random();

            do
            {
                xs = CopyComponents();

                while (xs.Count > 2)
                {
                    var n = rnd.Next() % componentConnections.Length;

                    var x1 = xs.Where(s => s.Contains(componentConnections[n].Item1)).First();
                    var x2 = xs.Where(s => s.Contains(componentConnections[n].Item2)).First();

                    if (x1 == x2) continue;

                    x1.AddRange(x2);

                    xs.Remove(x2);
                }

            } while (CutCount(xs) != 3);

            return xs;
        }

        private List<List<string>> CopyComponents()
        {
            List<List<string>> copy = [];

            foreach (var component in components) copy.Add([component]);

            return copy;
        }

        private int CutCount(List<List<string>> xs)
        {
            var c = 0;

            for (var n = 0; n < componentConnections.Length; ++n)
            {
                var x1 = xs.Where(s => s.Contains(componentConnections[n].Item1)).First();
                var x2 = xs.Where(s => s.Contains(componentConnections[n].Item2)).First();

                if (x1 != x2) ++c;
            }

            return c;
        }
    }
}
