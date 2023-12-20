using static AdventOfCode.Tests._2023.Day19.APlentyTests;

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
            // parser combinators
            // system.numerics.biginteger -> greatest common divisor

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
        public async Task AcceptedParts_CorrectlyIdentified_FronFile()
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


        public sealed record PartRatings(IReadOnlyDictionary<char, int> Ratings)
        {
            public static PartRatings Parse(string rawRatings)
            {
                rawRatings = rawRatings[1..^1];

                var ratings = rawRatings.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Split('='))
                                        .ToDictionary(a => a[0][0], a => int.Parse(a[1]));
                return new(ratings);
            }
        }

        public sealed class Workflows(IReadOnlyDictionary<string, Workflow> workflows)
        {
            public Workflow this[string name] => workflows[name];

            public static Workflows Parse(string rawWorkflows)
            {
                var rawWorkflow = rawWorkflows.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                var workflows = new Dictionary<string, Workflow> {
                    { "A", Workflow.Accepted },
                    { "R", Workflow.Rejected }
                };

                var ws = rawWorkflow.Select(rw => ParseWorkflow(rw, workflows)).ToDictionary(w => w.Name);

                foreach (var kvp in ws) workflows.Add(kvp.Key, kvp.Value);

                return new(workflows);
            }

            public static Workflow ParseWorkflow(string rawWorkflow, IReadOnlyDictionary<string, Workflow>? workflows = default)
            {
                var idx = rawWorkflow.IndexOf('{');

                var workflowName = rawWorkflow[..idx];

                var rawRules = rawWorkflow[(1 + idx)..^1].Split(',');

                var workflow = new Workflow(workflowName, rawWorkflow);

                workflow.ParseRules(rawRules, workflows);

                return workflow;
            }
        }

        public sealed class Workflow(string name, string rawWorkflow = default)
        {
            public static readonly Workflow Accepted = new("A");
            public static readonly Workflow Rejected = new("R");

            public string Raw = rawWorkflow;

            private readonly List<Rule> _rules = [];

            public string Name { get; init; } = name;

            public char Process(PartRatings partRatings)
            {
                if (Name == "A") return 'A';
                if (Name == "R") return 'R';
                
                for (var n = 0; n < _rules.Count; n++)
                {
                    var rule = _rules[n];

                    var eval = rule.Evaluate(partRatings);

                    if (eval.ConditionWasMet) return eval.Result;
                }

                return 'R';
            }

            public void ParseRules(IEnumerable<string> rawRules, IReadOnlyDictionary<string, Workflow>? workflows)
            {
                foreach (var rawRule in  rawRules)
                {
                    var rule = Rule.Parse(rawRule, workflows) with { Name = rawRule };

                    _rules.Add(rule);
                }
            }
        }

        public sealed record Condition(Predicate<PartRatings> IsSatisfied)
        {
            public readonly static Condition Satisfied = new(_ => true);

            public string Name { get; set; } = string.Empty;

            public static Condition Parse(string rawCondition)
            {
                var r = rawCondition[0];

                var op = rawCondition[1];
                
                var vl = int.Parse(rawCondition[2..]);

                return op switch
                {
                    '>' => new Condition(pr => pr.Ratings[r] > vl) with { Name = rawCondition },
                    '<' => new Condition(pr => pr.Ratings[r] < vl) with { Name = rawCondition },

                    _ => throw new ApplicationException($"Unexpected operator '{op}'")
                };
            }
        };

        public sealed record Rule(Condition Condition, Func<Workflow> ContinueWith)
        {
            public string Name { get; set; } = string.Empty;

            public (bool ConditionWasMet, char Result) Evaluate(PartRatings partRatings) =>
                Condition.IsSatisfied(partRatings)
                ? (true, ContinueWith().Process(partRatings))
                : (false, 'R');
            
            public static Rule Parse(string rawRule, IReadOnlyDictionary<string, Workflow>? workflows)
            {
                var s = rawRule.Split(':', StringSplitOptions.RemoveEmptyEntries);

                if (1 == s.Length) return new Rule(Condition.Satisfied, () => workflows[s[0]]);

                var condition = Condition.Parse(s[0]);

                var workflowToContinueWith = s[1];

                return new Rule(condition, () => workflows[workflowToContinueWith]);
            }
        }
    }
}
