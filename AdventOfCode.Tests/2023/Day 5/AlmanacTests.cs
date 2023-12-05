using AdventOfCode._2023.Day_5.SeedsAndFertiliser;

namespace AdventOfCode.Tests._2023.Day_5
{
    [TestClass]
    public sealed class AlmanacTests
    {
        private static readonly string[] rawAlmanac = [
            "seeds: 79 14 55 13",
            "",
            "seed-to-soil map:",
            "50 98 2",
            "52 50 48",
            "",
            "soil-to-fertilizer map:",
            "0 15 37",
            "37 52 2",
            "39 0 15",
            "",
            "fertilizer-to-water map:",
            "49 53 8",
            "0 11 42",
            "42 0 7",
            "57 7 4",
            "",
            "water-to-light map:",
            "88 18 7",
            "18 25 70",
            "",
            "light-to-temperature map:",
            "45 77 23",
            "81 45 19",
            "68 64 13",
            "",
            "temperature-to-humidity map:",
            "0 69 1",
            "1 0 69",
            "",
            "humidity-to-location map:",
            "60 56 37",
            "56 93 4",
        ];

        [DataTestMethod]
        [DataRow(79, 81)]
        [DataRow(14, 14)]
        [DataRow(55, 57)]
        [DataRow(13, 13)]
        public async Task Almanac_CorrectlyFindBestSoilTypeForSeed(long seedId, long expectedSoilId)
        {
            var rawAlmanac = AlmanacTests.rawAlmanac.ToAsyncEnumerable();

            var almanac = await Almanac.ParseAsync(rawAlmanac);

            var actualSoilId = almanac.TryFindInCategory(seedId, Almanac.Categories.SeedToSoil);

            Assert.AreEqual(expectedSoilId, actualSoilId);
        }

        [DataTestMethod]
        [DataRow(79, 82)]
        [DataRow(14, 43)]
        [DataRow(55, 86)]
        [DataRow(13, 35)]
        public async Task Almanac_CorrectlyFindsBestLocationForSeed(long seedId, long expectedLocationId)
        {
            var rawAlmanac = AlmanacTests.rawAlmanac.ToAsyncEnumerable();

            var almanac = await Almanac.ParseAsync(rawAlmanac);

            var soilId = almanac.TryFindInCategory(seedId, Almanac.Categories.SeedToSoil);
            var fertId = almanac.TryFindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
            var watrId = almanac.TryFindInCategory(fertId, Almanac.Categories.FertiliserToWater);
            var lghtId = almanac.TryFindInCategory(watrId, Almanac.Categories.WaterToLight);
            var tempId = almanac.TryFindInCategory(lghtId, Almanac.Categories.LightToTemperature);
            var humdId = almanac.TryFindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);

            var actualLocationId = almanac.TryFindInCategory(humdId, Almanac.Categories.HumidityToLocation);

            Assert.AreEqual(expectedLocationId, actualLocationId);
        }

        [DataTestMethod]
        [DataRow(35, 79, 14, 55, 13)]
        public async Task Almanac_CorrectlyFindsClosestLocationFromSeeds(long expectedLocationId, params long[] seedIds)
        {
            var rawAlmanac = AlmanacTests.rawAlmanac.ToAsyncEnumerable();

            var almanac = await Almanac.ParseAsync(rawAlmanac);

            var actualLocationId = long.MaxValue;

            foreach (var seedId in seedIds)
            {
                var soilId = almanac.TryFindInCategory(seedId, Almanac.Categories.SeedToSoil);
                var fertId = almanac.TryFindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
                var watrId = almanac.TryFindInCategory(fertId, Almanac.Categories.FertiliserToWater);
                var lghtId = almanac.TryFindInCategory(watrId, Almanac.Categories.WaterToLight);
                var tempId = almanac.TryFindInCategory(lghtId, Almanac.Categories.LightToTemperature);
                var humdId = almanac.TryFindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);
                var locnId = almanac.TryFindInCategory(humdId, Almanac.Categories.HumidityToLocation);

                if (locnId < actualLocationId) actualLocationId = locnId;
            }

            Assert.AreEqual(expectedLocationId, actualLocationId);
        }

        [TestMethod]
        public async Task Almanac_CorrectlyFindsClosestLocationFromSeedsInFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawAlmanac = File.ReadLinesAsync("2023/Day 5/Almanac-File.txt", cts.Token);

            var almanac = await Almanac.ParseAsync(rawAlmanac);

            var actualLocationId = long.MaxValue;

            foreach (var seedId in almanac.SeedsToBePlanted)
            {
                var soilId = almanac.TryFindInCategory(seedId, Almanac.Categories.SeedToSoil);
                var fertId = almanac.TryFindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
                var watrId = almanac.TryFindInCategory(fertId, Almanac.Categories.FertiliserToWater);
                var lghtId = almanac.TryFindInCategory(watrId, Almanac.Categories.WaterToLight);
                var tempId = almanac.TryFindInCategory(lghtId, Almanac.Categories.LightToTemperature);
                var humdId = almanac.TryFindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);
                var locnId = almanac.TryFindInCategory(humdId, Almanac.Categories.HumidityToLocation);

                if (locnId < actualLocationId) actualLocationId = locnId;
            }

            Assert.AreEqual(214922730, actualLocationId);
        }
    }
}
