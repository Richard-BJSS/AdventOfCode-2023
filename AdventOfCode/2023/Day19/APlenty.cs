namespace AdventOfCode._2023.Day19
{
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

            var workflow = new Workflow(workflowName);

            workflow.ParseRules(rawRules, workflows);

            return workflow;
        }
    }

    public sealed class Workflow(string name)
    {
        public static readonly Workflow Accepted = new("A");
        public static readonly Workflow Rejected = new("R");

        private readonly List<Rule> _rules = [];

        public string Name { get; init; } = name;

        public char Process(PartRatings partRatings)
        {
            if (Name == "A") return 'A';
            if (Name == "R") return 'R';

            for (var n = 0; n < _rules.Count; n++)
            {
                var rule = _rules[n];

                var (conditionWasMet, result) = rule.Evaluate(partRatings);

                if (conditionWasMet) return result;
            }

            return 'R';
        }

        public long CountOfAcceptableRatings(Workflows workflows, Dictionary<char, (int Min, int Max)> rangedPartRatings)
        {
            if (Name == "R") return 0L;
            if (Name == "A") return rangedPartRatings.Values.Aggregate(1L, (product, range) => product * (range.Max - range.Min + 1));

            foreach (var rule in _rules)
            {
                var workflow = workflows[rule.Workflow];

                var nvr = rule.Condition.ValidRatings;

                if (nvr is null) return workflow.CountOfAcceptableRatings(workflows, rangedPartRatings);

                var (rating, min, max) = nvr.Value;

                var original = rangedPartRatings[rating];

                var accepted = (Min: Math.Max(min, original.Min), Max: Math.Min(max, original.Max));

                var conditionIsMet = accepted.Max - accepted.Min > 0;

                if (!conditionIsMet) continue;

                var acceptedCount = 0L;

                if (accepted.Max - accepted.Min > 0)
                {
                    var acceptedBranch = new Dictionary<char, (int, int)>(rangedPartRatings) { [rating] = accepted };

                    acceptedCount = workflow.CountOfAcceptableRatings(workflows, acceptedBranch);
                }

                var rejections = CalculateRejectedRange(original, accepted);

                if (rejections.Max - rejections.Min > 0)
                {
                    var rejectionBranch = new Dictionary<char, (int, int)>(rangedPartRatings) { [rating] = rejections };

                    acceptedCount += CountOfAcceptableRatings(workflows, rejectionBranch);
                }

                return acceptedCount;
            }

            return 0L;
        }

        public void ParseRules(IEnumerable<string> rawRules, IReadOnlyDictionary<string, Workflow>? workflows)
        {
            foreach (var rawRule in rawRules)
            {
                var rule = Rule.Parse(rawRule, workflows);

                _rules.Add(rule);
            }
        }

        private static (int Min, int Max) CalculateRejectedRange((int Min, int Max) original, (int Min, int Max) accepted)
            => accepted.Min == original.Min
               ? (accepted.Max + 1, original.Max)
               : (original.Min, accepted.Min - 1);

        //{
        //    if (accepted.Min == original.Min)
        //    {
        //        // <
        //        var x = accepted.Max + 1;

        //        return (x, original.Max);
        //    }
        //    else
        //    {
        //        // >
        //        var x = accepted.Min - 1;

        //        return (original.Min, x);
        //    }
        //}
    }

    public record struct Condition(Predicate<PartRatings> IsSatisfied)
    {
        public readonly static Condition Satisfied = new(_ => true);

        public (char Rating, int Min, int Max)? ValidRatings { get; private set; }

        public static Condition Parse(string rawCondition)
        {
            var r = rawCondition[0];

            var op = rawCondition[1];

            var vl = int.Parse(rawCondition[2..]);

            return op switch
            {
                '>' => new Condition(pr => pr.Ratings[r] > vl) with { ValidRatings = (r, vl + 1, int.MaxValue) },
                '<' => new Condition(pr => pr.Ratings[r] < vl) with { ValidRatings = (r, int.MinValue, vl - 1) },

                _ => throw new ApplicationException($"Unexpected operator '{op}'")
            };
        }
    };

    public record struct Rule(Condition Condition, Func<Workflow> ContinueWith, string Workflow)
    {
        public (bool ConditionWasMet, char Result) Evaluate(PartRatings partRatings) =>
            Condition.IsSatisfied(partRatings)
            ? (true, ContinueWith().Process(partRatings))
            : (false, 'R');

        public static Rule Parse(string rawRule, IReadOnlyDictionary<string, Workflow>? workflows)
        {
            var s = rawRule.Split(':', StringSplitOptions.RemoveEmptyEntries);

            if (1 == s.Length) return new Rule(Condition.Satisfied, () => workflows[s[0]], s[0]);

            var condition = Condition.Parse(s[0]);

            var workflowToContinueWith = s[1];

            return new Rule(condition, () => workflows[workflowToContinueWith], workflowToContinueWith);
        }
    }
}
