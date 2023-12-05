using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day_5.SeedsAndFertiliser
{
    public partial class Almanac(long[] seedsToBePlanted, long[][][] categoryMaps)
    {
        public enum Categories
        {
            SeedToSoil = 0, 
            SoilToFertiliser = 1,
            FertiliserToWater = 2,
            WaterToLight = 3,
            LightToTemperature = 4,
            TemperatureToHumidity = 5,
            HumidityToLocation = 6,
        }

        [GeneratedRegex("[0-9]+", RegexOptions.Compiled)] private static partial Regex _regex();

        public long[] SeedsToBePlanted => seedsToBePlanted;

        public static async Task<Almanac> ParseAsync(IAsyncEnumerable<string> rawAlmanac)
        {
            var seeds = Array.Empty<long>();

            var maps = new long[7][][];

            for (var n = 0; n < maps.Length; n++) maps[n] = new long[3][];

            int x = 0, y= 0;

            await foreach(var rawRow in rawAlmanac)
            {
                if (string.IsNullOrWhiteSpace(rawRow)) continue;

                if (rawRow.StartsWith("seeds: "))
                {
                    seeds = rawRow[7..].Split(' ').Select(long.Parse).ToArray();
                    continue;
                }
                else if (rawRow == "seed-to-soil map:") { x = 0; y = 0; }
                else if (rawRow == "soil-to-fertilizer map:") { x = 1; y = 0; }
                else if (rawRow == "fertilizer-to-water map:") { x = 2; y = 0; }
                else if (rawRow == "water-to-light map:") { x = 3; y = 0; }
                else if (rawRow == "light-to-temperature map:") { x = 4; y = 0; }
                else if (rawRow == "temperature-to-humidity map:") { x = 5; y = 0; }
                else if (rawRow == "humidity-to-location map:") { x = 6; y = 0; }
                else {
                    var ms = _regex().Matches(rawRow).Select(m => long.Parse(m.Value)).ToArray();

                    if (3 != ms.Length) throw new ApplicationException($"The 3 column numeric mapping row is in an unexpected format : {rawRow}");

                    if (y % 5 == 0) {
                        Array.Resize(ref maps[x][0], y + 5);
                        Array.Resize(ref maps[x][1], y + 5);
                        Array.Resize(ref maps[x][2], y + 5);
                    }

                    maps[x][0][y] = ms[0];
                    maps[x][1][y] = ms[1];
                    maps[x][2][y] = ms[2];

                    y++;
                }
            }

            return new(seeds,maps);
        }

        public long TryFindInCategory(long id, Categories category)
        {
            var x = (int)category;

            var destinationStartIds = categoryMaps[x][0];
            var sourceStartIds = categoryMaps[x][1];
            var rangeLengths = categoryMaps[x][2];

            for (var n = 0; n < sourceStartIds.Length; n++)
            {
                var rangeLength = rangeLengths[n];

                if (0 == rangeLength) break;

                var sourceStartId = sourceStartIds[n];
                var destinationStartId = destinationStartIds[n];

                if (sourceStartId == id) return destinationStartIds[n];

                if (sourceStartId > id) continue; 

                if (sourceStartId + rangeLength < id) continue;

                return destinationStartId + (id - sourceStartId);
            }

            return id;
        }
    }
}
