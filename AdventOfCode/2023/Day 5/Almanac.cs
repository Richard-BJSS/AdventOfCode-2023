using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day_5.SeedsAndFertiliser
{

    // Looks like we're reaching the limits of what we can accomplish with brute force calculations.
    // Probably worth revisiting this one to use interval logic instead, which will be more difficult to
    // read (if you are unfmailiar with the maths) but much faster to execute.
    //
    // Presume in the days ahead I'll have to code up some useful types/alogs that can be retrofitted to this 
    // solution when time allows.  Not sure if System.Range can be used so will explore

    public partial class Almanac((long StartingSeedId, long Range)[] seedsToBePlanted, long[][][] categoryMaps)
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

        public static Task<Almanac> ParseV1Async(IAsyncEnumerable<string> rawAlmanac) => ParseAsync(rawAlmanac, ParseSeedsV1);
        public static Task<Almanac> ParseV2Async(IAsyncEnumerable<string> rawAlmanac) => ParseAsync(rawAlmanac, ParseSeedsV2);

        private static async Task<Almanac> ParseAsync(IAsyncEnumerable<string> rawAlmanac, Func<string, (long, long)[]> parser)
        {
            var seeds = default((long, long)[]);

            var maps = new long[7][][];

            for (var n = 0; n < maps.Length; n++) maps[n] = new long[3][];

            int x = 0, y= 0;

            await foreach(var rawRow in rawAlmanac)
            {
                if (string.IsNullOrWhiteSpace(rawRow)) continue;

                if (rawRow.StartsWith("seeds: "))
                {
                    seeds = parser(rawRow[7..]);
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

            return new(seeds, maps);
        }

        private static (long, long)[] ParseSeedsV1(string rawSeeds) => rawSeeds.Split(' ').Select(s => (long.Parse(s), 1L)).ToArray();

        private static (long, long)[] ParseSeedsV2(string rawSeeds) => rawSeeds.Split(' ').Chunk(2).Select(c => (long.Parse(c[0]), long.Parse(c[1]))).ToArray();

        public long FindInCategory(long id, Categories category)
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

        public long FindClosestLocationFromSeeds()
        {
            var actualLocationId = long.MaxValue;

            var seedIds = YieldSeedIds(seedsToBePlanted);

            object lck = new();

            foreach (var seedId in seedIds.AsParallel())
            {
                var soilId = FindInCategory(seedId, Almanac.Categories.SeedToSoil);
                var fertId = FindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
                var watrId = FindInCategory(fertId, Almanac.Categories.FertiliserToWater);
                var lghtId = FindInCategory(watrId, Almanac.Categories.WaterToLight);
                var tempId = FindInCategory(lghtId, Almanac.Categories.LightToTemperature);
                var humdId = FindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);
                var locnId = FindInCategory(humdId, Almanac.Categories.HumidityToLocation);

                lock (lck) { if (locnId < actualLocationId) actualLocationId = locnId; }
            }

            return actualLocationId;
        }

        private static IEnumerable<long> YieldSeedIds((long, long)[] seedsToBePlanted)
        {
            foreach (var (seedId, length) in seedsToBePlanted)
                for (var n = 0L; n < length; n++) yield return seedId + n;
        }
    }
}
