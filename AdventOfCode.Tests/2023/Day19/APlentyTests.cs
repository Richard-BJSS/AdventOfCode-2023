using AdventOfCode._2023.Day19;
using System.Data;

namespace AdventOfCode.Tests._2023.Day19
{
    [TestClass]
    public sealed class APlentyTests
    {
        [DataTestMethod]
        [DataRow(0, "in{x>10:xw,m<20:mw,a>30:aw,s<15:sw,R}\r\nxw{A}\r\nmw{A}\r\naw{A}\r\nsw{A}\r\n\r\n{x=1,m=21,a=30,s=15}")]
        [DataRow(14, "in{x>10:xw,m<20:mw,a>30:aw,s<15:sw,R}\r\nxw{A}\r\nmw{A}\r\naw{A}\r\nsw{A}\r\n\r\n{x=11,m=1,a=1,s=1}")]
        [DataRow(22, "in{x>10:xw,m<20:mw,a>30:aw,s<15:sw,R}\r\nxw{A}\r\nmw{A}\r\naw{A}\r\nsw{A}\r\n\r\n{x=1,m=19,a=1,s=1}")]
        [DataRow(34, "in{x>10:xw,m<20:mw,a>30:aw,s<15:sw,R}\r\nxw{A}\r\nmw{A}\r\naw{A}\r\nsw{A}\r\n\r\n{x=1,m=1,a=31,s=1}")]
        [DataRow(15, "in{x>10:xw,m<20:mw,a>30:aw,s<15:sw,R}\r\nxw{A}\r\nmw{A}\r\naw{A}\r\nsw{A}\r\n\r\n{x=1,m=1,a=1,s=12}")]
        [DataRow(19114, "px{a<2006:qkq,m>2090:A,rfg}\r\npv{a>1716:R,A}\r\nlnx{m>1548:A,A}\r\nrfg{s<537:gd,x>2440:R,A}\r\nqs{s>3448:A,lnx}\r\nqkq{x<1416:A,crn}\r\ncrn{x>2662:A,R}\r\nin{s<1351:px,qqz}\r\nqqz{s>2770:qs,m<1801:hdj,R}\r\ngd{a>3333:R,R}\r\nhdj{m>838:A,pv}\r\n\r\n{x=787,m=2655,a=1222,s=2876}\r\n{x=1679,m=44,a=2067,s=496}\r\n{x=2036,m=264,a=79,s=2244}\r\n{x=2461,m=1339,a=466,s=291}\r\n{x=2127,m=1623,a=2188,s=1013}")]
        public void AcceptedParts_CorrectlyIdentified(int expectedOutcome, string rawWorkflowsAndRatings)
        {
            var rawSchemas = rawWorkflowsAndRatings.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var rawPartRatings = rawSchemas[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var partsWithRatings = rawPartRatings.Select(PartRatings.Parse).ToArray();

            var workflows = Workflows.Parse(rawSchemas[0]);

            var workflow = workflows["in"];

            var actualOutcome = 0;

            for (var n = 0; n < partsWithRatings.Length; n++)
            {
                var partRatings = partsWithRatings[n];

                if (workflow.Process(partRatings) == 'A')
                {
                    actualOutcome += partRatings.Ratings.Values.Sum();
                }
            }

            Assert.AreEqual(expectedOutcome, actualOutcome);
        }

        [TestMethod]
        public void AcceptedRatingCombinations_CorrectlyIdentified_UsingRanges()
        {
            var rawWorkflowsAndRatings = "px{a<2006:qkq,m>2090:A,rfg}\r\npv{a>1716:R,A}\r\nlnx{m>1548:A,A}\r\nrfg{s<537:gd,x>2440:R,A}\r\nqs{s>3448:A,lnx}\r\nqkq{x<1416:A,crn}\r\ncrn{x>2662:A,R}\r\nin{s<1351:px,qqz}\r\nqqz{s>2770:qs,m<1801:hdj,R}\r\ngd{a>3333:R,R}\r\nhdj{m>838:A,pv}";

            var rawSchemas = rawWorkflowsAndRatings.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var workflows = Workflows.Parse(rawSchemas[0]);

            var workflow = workflows["in"];

            var rangedPartRatings = new Dictionary<char, (int Min, int Max)>() { { 'x', (1, 4000) }, { 'm', (1, 4000) }, { 'a', (1, 4000) }, { 's', (1, 4000) }};

            var actualCount = workflow.CountOfAcceptableRatings(workflows, rangedPartRatings);

            Assert.AreEqual(167409079868000, actualCount);
        }

        [TestMethod]
        public async Task AcceptedParts_CorrectlyIdentified_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawWorkflowsAndRatings = await File.ReadAllTextAsync("2023/Day19/A-Plenty-File.txt", cts.Token);

            var rawSchemas = rawWorkflowsAndRatings.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var rawPartRatings = rawSchemas[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var partsWithRatings = rawPartRatings.Select(PartRatings.Parse).ToArray();

            var workflows = Workflows.Parse(rawSchemas[0]);

            var workflow = workflows["in"];

            var actualOutcome = 0;

            for (var n = 0; n < partsWithRatings.Length; n++)
            {
                var partRatings = partsWithRatings[n];

                if (workflow.Process(partRatings) == 'A')
                {
                    actualOutcome += partRatings.Ratings.Values.Sum();
                }
            }

            Assert.AreEqual(532551, actualOutcome);
        }

        [TestMethod]
        public async Task AcceptedRatingCombinations_CorrectlyIdentified_UsingRanges_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawWorkflowsAndRatings = await File.ReadAllTextAsync("2023/Day19/A-Plenty-File.txt", cts.Token);

            var rawSchemas = rawWorkflowsAndRatings.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            var workflows = Workflows.Parse(rawSchemas[0]);

            var workflow = workflows["in"];

            var rangedPartRatings = new Dictionary<char, (int Min, int Max)>() { { 'x', (1, 4000) }, { 'm', (1, 4000) }, { 'a', (1, 4000) }, { 's', (1, 4000) } };

            var actualCount = workflow.CountOfAcceptableRatings(workflows, rangedPartRatings);

            Assert.AreEqual(134343280273968, actualCount);
        }
    }
}
