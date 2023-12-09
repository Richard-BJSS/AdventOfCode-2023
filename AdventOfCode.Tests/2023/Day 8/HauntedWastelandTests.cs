using AdventOfCode._2023.Day_8.HauntedWasteland;

namespace AdventOfCode.Tests._2023.Day_8
{
    [TestClass]
    public sealed class HauntedWastelandTests
    {
        [DataTestMethod]
        [DataRow("RL", 2)]
        public async Task CountSteps_CalculatedCorrectly(string stepPattern, int expectedStepCount)
        {
            var network = new Network(new Dictionary<Location, (Location Left, Location Right)>
            {
                ["AAA"] = ("BBB", "CCC"),
                ["BBB"] = ("DDD", "EEE"),
                ["CCC"] = ("ZZZ", "GGG"),
                ["DDD"] = ("DDD", "DDD"),   
                ["EEE"] = ("EEE", "EEE"),   
                ["GGG"] = ("GGG", "GGG"),
                ["ZZZ"] = ("ZZZ", "ZZZ"),
            });

            var startingLocation = network.Locations.Single(l => l.Identifier.EndsWith("AAA"));
            
            var journey = network.StartJourney(stepPattern, startingLocation);

            await journey.EndWhenAsync(l => l.Identifier == "ZZZ");

            var stepCount = journey.Tick;

            Assert.AreEqual(expectedStepCount, stepCount);
        }

        [DataTestMethod]
        [DataRow("LLR", 6)]
        public async Task CountSteps_CalculatedCorrectly_LongerPattern(string stepPattern, int expectedStepCount)
        {
            var network = new Network(new Dictionary<Location, (Location Left, Location Right)>
            {
                ["AAA"] = ("BBB", "BBB"),
                ["BBB"] = ("AAA", "ZZZ"),
                ["ZZZ"] = ("ZZZ", "ZZZ"), 
            });

            var startingLocation = network.Locations.Single(l => l.Identifier.EndsWith("AAA"));

            var journey = network.StartJourney(stepPattern, startingLocation);

            await journey.EndWhenAsync(l => l.Identifier == "ZZZ");

            var stepCount = journey.Tick;

            Assert.AreEqual(expectedStepCount, stepCount);

            Assert.AreEqual(expectedStepCount, stepCount);
        }

        [DataTestMethod]
        [DataRow("AAA", "ZZZ", 16697)]
        public async Task CountSteps_CalculatedCorrectlyFromFile(string startFrom, string endAt, int expectedStepCount)
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var raw = await File.ReadAllLinesAsync("2023/Day 8/Haunted-Wasteland-File.txt", cts.Token);

            var stepPattern = raw[0];

            var network = Network.Parse(raw.Skip(2));

            var startingLocation = network.Locations.Single(l => l.Identifier == startFrom);

            var journey = network.StartJourney(stepPattern, startingLocation);

            await journey.EndWhenAsync(l => l.Identifier == endAt);

            var stepCount = journey.Tick;

            Assert.AreEqual(expectedStepCount, stepCount);
        }

        [TestMethod]
        public async Task CountSteps_CalculatesCorrectlyInParallel()
        {
            var raw = new[] {
                "11A = (11B, XXX)",
                "11B = (XXX, 11Z)",
                "11Z = (11B, XXX)",
                "22A = (22B, XXX)",
                "22B = (22C, 22C)",
                "22C = (22Z, 22Z)",
                "22Z = (22B, 22B)",
                "XXX = (XXX, XXX)"
            };

            var network = Network.Parse(raw);

            var startingLocations = network.Locations.Where(l => l.Identifier.EndsWith('A'));

            var journeys = startingLocations.Select(l => network.StartJourney("LR", l)).ToArray();

            var signals = journeys.Select(j => j.EndWhenAsync(l => l.Identifier.EndsWith('Z'))).AsParallel();

            await Task.WhenAll(signals);

            var stepCount = journeys.Select(j => j.Tick).LowestCommonMultiple();

            Assert.AreEqual(6, stepCount);
        }

        [TestMethod]
        public async Task CountSteps_CalculatesCorrectlyInParallelFromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var raw = await File.ReadAllLinesAsync("2023/Day 8/Haunted-Wasteland-File.txt", cts.Token);

            var stepPattern = raw[0];

            var network = Network.Parse(raw.Skip(2));

            var startingLocations = network.Locations.Where(l => l.Identifier.EndsWith('A'));

            var journeys = startingLocations.Select(l => network.StartJourney(stepPattern, l)).ToArray();

            var signals = journeys.Select(j => j.EndWhenAsync(l => l.Identifier.EndsWith('Z'))).AsParallel();
        
            await Task.WhenAll(signals);

            var stepCount = journeys.Select(j => j.Tick).LowestCommonMultiple();

            Assert.AreEqual(10668805667831, stepCount);
        }
    }
}