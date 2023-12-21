using System.Text.RegularExpressions;

namespace AdventOfCode._2023.Day20
{
    public sealed record RxModule() : Module("rx", Array.Empty<string>())
    {
        private readonly HashSet<Module> _inboundModules = [];

        public IReadOnlyDictionary<string, List<long>> Counters { get; set; }

        public void RegisterInboundModule(Module module) => _inboundModules.Add(module);

        public IEnumerable<Module> InboundModules => _inboundModules;
    }

    public sealed record Broadcaster(string Name, IEnumerable<string> Destinations) : Module(Name, Destinations)
    {
        public override bool? BroadcastHighPulse(Pulse pulse) => pulse.IsHigh;
    }

    public sealed record FlipFlop(string Name, IEnumerable<string> Destinations) : Module(Name[1..], Destinations)
    {
        private readonly Dictionary<string, bool> _memory = [];

        private bool _isOn = false;

        public override bool? BroadcastHighPulse(Pulse pulse)
        {
            if (pulse.IsHigh) return null;

            _isOn = !_isOn;

            return _isOn;
        }
    }

    public sealed record Conjunction(string Name, IEnumerable<string> Destinations) : Module(Name[1..], Destinations)
    {
        private readonly Dictionary<string, bool> _memory = [];

        public override bool? BroadcastHighPulse(Pulse pulse)
        {
            var sourceName = pulse.Source?.Name ?? "Button";

            _memory[sourceName] = pulse.IsHigh;

            return _memory.Values.Any(_ => _ == false);
        }

        public bool ValueFor(Module module) => _memory[module.Name];

        public void ResetMemory(IEnumerable<Module> inboundModules)
        {
            _memory.Clear();

            foreach (var module in inboundModules) _memory[module.Name] = false;
        }
    }

    public record Module(string Name, IEnumerable<string> ModulesToForwardOnTo)
    {
        public virtual bool? BroadcastHighPulse(Pulse pulse) => default;
    };

    public readonly record struct Pulse(bool IsHigh, Module Source, Module Destination)
    {
        public override string ToString() => $"{Source.Name} [{Source.GetType().Name}] send {(IsHigh ? "High" : "Low")} signal to {Destination.Name} [{Destination.GetType().Name}]";
    }

    public record struct SimulationResult(IReadOnlyDictionary<bool, long> Pulses, RxModule? Rx);

    public sealed class Network(IDictionary<string, Module> Modules)
    {
        public static async Task<Network> ParseAsync(IAsyncEnumerable<string> rawModules)
        {
            var regex = new Regex("(?<Id>[%&a-z]+) -> (?:(?<DestMods>[a-z]+)(?:, )?)+", RegexOptions.Compiled);

            var modules = new Dictionary<string, Module>();

            await foreach (var rawModule in rawModules)
            {
                var ms = regex.Match(rawModule);

                var destinationModules = ms.Groups["DestMods"].Captures.Select(c => c.Value).ToArray();

                var moduleIdentifier = ms.Groups["Id"].Value;

                Module module = moduleIdentifier[0] switch
                {
                    '%' => new FlipFlop(moduleIdentifier, destinationModules),
                    '&' => new Conjunction(moduleIdentifier, destinationModules),
                    _ => new Broadcaster(moduleIdentifier, destinationModules),
                };

                modules[module.Name] = module with { ModulesToForwardOnTo = destinationModules };
            }

            foreach (var con in modules.Values.OfType<Conjunction>())
            {
                con.ResetMemory(modules.Values.Where(m => m.ModulesToForwardOnTo.Contains(con.Name)));
            }

            return new(modules);
        }

        public SimulationResult Simulate(int numOfButtonPresses)
        {
            var pulseCounter = new Dictionary<bool, long>(2) { [false] = 0, [true] = 0 };

            var counters = new Dictionary<string, List<long>>();

            var button = new Module("Button", Array.Empty<string>());

            var rxModule = default(RxModule);

            if (Modules.TryGetValue("rx", out var rx)) rxModule = (RxModule)rx;

            for (var btn = 0; btn < numOfButtonPresses; btn++)
            {
                var pulses = new Queue<Pulse>([new(false, button, Modules["broadcaster"])]);

                while (0 < pulses.Count)
                {
                    var pulse = pulses.Dequeue();

                    pulseCounter[pulse.IsHigh]++;

                    if (pulse.IsHigh && pulse.Destination is Conjunction con && !con.ValueFor(pulse.Source))
                    {
                        if (!counters.TryGetValue(con.Name, out var cntrs)) counters[con.Name] = cntrs = [];

                        counters[con.Name].Add(btn + 1);
                    }

                    var broadcastHighPulse = pulse.Destination.BroadcastHighPulse(pulse);

                    if (broadcastHighPulse is null) continue;

                    foreach (var moduleToForwardTo in pulse.Destination.ModulesToForwardOnTo)
                    {
                        var isRx = moduleToForwardTo == "rx";

                        if (!Modules.TryGetValue(moduleToForwardTo, out var module))
                        {
                            Modules[moduleToForwardTo] = module =
                                isRx
                                ? rxModule = new RxModule()
                                : new Module(moduleToForwardTo, Array.Empty<string>());
                        }

                        if (isRx) rxModule?.RegisterInboundModule(pulse.Source);

                        var nextPulse = new Pulse(broadcastHighPulse.Value, pulse.Destination, module);

                        pulses.Enqueue(nextPulse);
                    }
                }
            }

            if (rxModule is not null) rxModule.Counters = counters;

            return new(pulseCounter, rxModule);
        }
    }
}
