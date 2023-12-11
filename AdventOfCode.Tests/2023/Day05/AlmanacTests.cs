using AdventOfCode._2023.Day05.SeedsAndFertiliser;

namespace AdventOfCode.Tests._2023.Day05
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

            var almanac = await Almanac.ParseV1Async(rawAlmanac);

            var actualSoilId = almanac.FindInCategory(seedId, Almanac.Categories.SeedToSoil);

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

            var almanac = await Almanac.ParseV1Async(rawAlmanac);

            var soilId = almanac.FindInCategory(seedId, Almanac.Categories.SeedToSoil);
            var fertId = almanac.FindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
            var watrId = almanac.FindInCategory(fertId, Almanac.Categories.FertiliserToWater);
            var lghtId = almanac.FindInCategory(watrId, Almanac.Categories.WaterToLight);
            var tempId = almanac.FindInCategory(lghtId, Almanac.Categories.LightToTemperature);
            var humdId = almanac.FindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);

            var actualLocationId = almanac.FindInCategory(humdId, Almanac.Categories.HumidityToLocation);

            Assert.AreEqual(expectedLocationId, actualLocationId);
        }

        [DataTestMethod]
        [DataRow(35, 79, 14, 55, 13)]
        public async Task Almanac_CorrectlyFindsClosestLocationFromSeeds(long expectedLocationId, params long[] seedIds)
        {
            var rawAlmanac = AlmanacTests.rawAlmanac.ToAsyncEnumerable();

            var almanac = await Almanac.ParseV1Async(rawAlmanac);

            var actualLocationId = long.MaxValue;

            foreach (var seedId in seedIds)
            {
                var soilId = almanac.FindInCategory(seedId, Almanac.Categories.SeedToSoil);
                var fertId = almanac.FindInCategory(soilId, Almanac.Categories.SoilToFertiliser);
                var watrId = almanac.FindInCategory(fertId, Almanac.Categories.FertiliserToWater);
                var lghtId = almanac.FindInCategory(watrId, Almanac.Categories.WaterToLight);
                var tempId = almanac.FindInCategory(lghtId, Almanac.Categories.LightToTemperature);
                var humdId = almanac.FindInCategory(tempId, Almanac.Categories.TemperatureToHumidity);
                var locnId = almanac.FindInCategory(humdId, Almanac.Categories.HumidityToLocation);

                if (locnId < actualLocationId) actualLocationId = locnId;
            }

            Assert.AreEqual(expectedLocationId, actualLocationId);
        }

        [TestMethod]
        public async Task Almanac_CorrectlyFindsClosestLocationV1FromSeedsInFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawAlmanac = File.ReadLinesAsync("2023/Day05/Almanac-File.txt", cts.Token);

            var almanac = await Almanac.ParseV1Async(rawAlmanac);

            var actualLocationId = almanac.FindClosestLocationFromSeeds();

            Assert.AreEqual(214922730, actualLocationId);
        }

        [TestMethod]
        public async Task Almanac_CorrectlyFindsClosestLocationV2FromSeedsInFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawAlmanac = File.ReadLinesAsync("2023/Day05/Almanac-File.txt", cts.Token);

            var almanac = await Almanac.ParseV2Async(rawAlmanac);

            var actualLocationId = almanac.FindClosestLocationFromSeeds();

            Assert.AreEqual(148041808, actualLocationId);
        }
    }
}
