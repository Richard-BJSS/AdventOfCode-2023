using AdventOfCode.Maths.Geometry.Euclidean;
using System.Drawing;

namespace AdventOfCode._2023.Day21
{
    public static class StepCounter
    {
        public static IEnumerable<long> LocateAccessibleGardenPlotsAtDistances(Point startAt, Func<Point, IEnumerable<Point>> getAdjGardenPlots, params int[] respondWhenDistanceTravelledIsIn)
        {
            var plotsToCheck = new HashSet<Point>([startAt]);

            var gardenPlots = new HashSet<Point>();

            var maxDistanceToTravel = respondWhenDistanceTravelledIsIn.Max();

            for (var distanceTravelled = 1; distanceTravelled <= maxDistanceToTravel; distanceTravelled++)
            {
                gardenPlots.Clear();

                foreach (var plot in plotsToCheck)
                {
                    var adjs = getAdjGardenPlots(plot);

                    foreach (var adj in adjs) gardenPlots.Add(adj);
                }

                if (respondWhenDistanceTravelledIsIn.Contains(distanceTravelled)) yield return gardenPlots.Count;

                (plotsToCheck, gardenPlots) = (gardenPlots, plotsToCheck);
            }
        }

        public static Func<Point, IEnumerable<Point>> GetAdjacentGardenPlots(Matrix<char> matrix)
        {
            const char GARDEN_PLOT = '.';

            var periodicMatrix = matrix.ToPeriodicMatrix();

            Func<Point, IEnumerable<Point>> f =
                plot => periodicMatrix.PointsAdjacentTo(plot, Compass.News)
                                      .Select(adj => adj.Point)
                                      .Where(plot => periodicMatrix[plot] == GARDEN_PLOT)
                                      .ToArray();

            return f.Memoize();
        }
    }
}
