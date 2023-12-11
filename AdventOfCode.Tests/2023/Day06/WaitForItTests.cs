using AdventOfCode._2023.Day06;

namespace AdventOfCode.Tests._2023.Day06
{
    [TestClass]
    public sealed class WaitForItTests
    {
        [DataTestMethod]
        [DataRow("Time: 7 15 30",     "Distance: 9 40 200",              288)]
        [DataRow("Time: 44 70 70 80", "Distance: 283 1134 1134 1491", 219849)]
        [DataRow("Time: 71530",       "Distance: 940200",              71503)]
        [DataRow("Time: 44707080",    "Distance: 283113411341491",  29432455)]
        public void Calculates_MarginOfErrorCorrectly(string rawTime, string rawDistance, int expectedMarginOfError)
        {
            var rawData = new[] { rawTime, rawDistance };

            var raceRecords = WaitForIt.Parse(rawData);

            var marginOfError = 1L;

            var countOfRaces = raceRecords.GetLength(0);

            for (var race = 0; race < countOfRaces; race++)
            {
                var raceDurationMs = raceRecords[race, 0];
                var raceDistCoveredMm = raceRecords[race, 1];

                var c = WaitForIt.TickingClock(raceDurationMs);

                var x = c
                    .Select(c => (startingSpeed: c, distance: c * (raceDurationMs - c)))
                    .Where(r => r.distance > raceDistCoveredMm)
                    .Select(r => r.startingSpeed)
                    .LongCount();
                    ;

                marginOfError *= (x == 0) ? 1 : x;
            }

            Assert.AreEqual(expectedMarginOfError, marginOfError);
        }

        [DataTestMethod]
        [DataRow("Time: 7 15 30", "Distance: 9 40 200", 288)]
        [DataRow("Time: 44 70 70 80", "Distance: 283 1134 1134 1491", 219849)]
        [DataRow("Time: 71530", "Distance: 940200", 71503)]
        [DataRow("Time: 44707080", "Distance: 283113411341491", 29432455)]
        public void Calculates_MarginOfErrorCorrectly_MathifiedForGiggles(string rawTime, string rawDistance, int expectedMarginOfError)
        {
            // d = c * (t - c) where d is distance covered, c is initial charge, and t is time taken
            // d = -c² + ct
            // formulae forms a quadratic curve, so we can use it to calculate those distances travelled that 
            // exceed the previous record ...
            // i.e. where (-c² + ct - d > 0) holds true

            // quadratic equation : (-b ± √(b² - 4ac)) / 2a

            var rawData = new[] { rawTime, rawDistance };

            var raceRecords = WaitForIt.Parse(rawData);

            var marginOfError = 1L;

            var countOfRaces = raceRecords.GetLength(0);

            for (var race = 0; race < countOfRaces; race++)
            {
                var raceDuration = raceRecords[race, 0];  // race duration in ms
                var raceRecord = raceRecords[race, 1];    // race distance record in mm

                var chargeTime1 = (-raceDuration + Math.Sqrt((raceDuration * raceDuration) - (4 * raceRecord))) / -2;
                var chargeTime2 = (-raceDuration - Math.Sqrt((raceDuration * raceDuration) - (4 * raceRecord))) / -2;

                // range = lowest charge time to highest

                if (chargeTime1 > chargeTime2) (chargeTime2, chargeTime1) = (chargeTime1, chargeTime2);   

                // adjust to consider we only want distances travelled that break the existing record, and we have calculated
                // exact values on the curve

                var rs = (long)Math.Ceiling((0 == chargeTime1 % 1) ? ++chargeTime1 : chargeTime1);
                var re = (long)Math.Floor(  (0 == chargeTime2 % 1) ? --chargeTime2 : chargeTime2);

                var c = (0 >= (re - rs)) ? 1 : ((re - rs) + 1);

                marginOfError *= c;
            }

            Assert.AreEqual(expectedMarginOfError, marginOfError);
        }
    }
}
