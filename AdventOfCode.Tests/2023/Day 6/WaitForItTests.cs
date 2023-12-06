using AdventOfCode._2023.Day_6;

namespace AdventOfCode.Tests._2023.Day_6
{
    [TestClass]
    public sealed class WaitForItTests
    {
        [DataTestMethod]
        [DataRow("Time: 7 15 30",     "Distance: 9 40 200",              288)]
        [DataRow("Time: 44 70 70 80", "Distance: 283 1134 1134 1491", 219849)]
        [DataRow("Time: 71530",       "Distance: 940200",              71503)]
        [DataRow("Time: 44707080",    "Distance: 283113411341491",  29432455)]
        public async Task Do(string rawTime, string rawDistance, int expectedMarginOfError)
        {
            var rawData = new[] { rawTime, rawDistance };

            var raceRecords = WaitForIt.Parse(rawData);
            var marginOfError = 1L;

            var countOfRaces = raceRecords.GetLength(0);

            for (var race = 0; race < countOfRaces; race++)
            {
                var timeTakenMs = raceRecords[race, 0];
                var distCoveredMm = raceRecords[race, 1];

                var clock = WaitForIt.TickingClock(timeTakenMs);

                var x = clock
                    .Select(t => (startingSpeed: t, distance: t * (timeTakenMs - t)))
                    .Where(r => r.distance > distCoveredMm)
                    .Select(r => r.startingSpeed)
                    .LongCount();
                    ;

                marginOfError *= (x == 0) ? 1 : x;
            }

            Assert.AreEqual(expectedMarginOfError, marginOfError);
        }
    }
}
