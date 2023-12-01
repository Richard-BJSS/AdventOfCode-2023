using AdventOfCode._2023.Day_1;

using var cts = new CancellationTokenSource(delay: TimeSpan.FromMinutes(5));

Console.WriteLine("Running 2023 - Day 1 - Part 1 : Trebuchet");

var xs = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

var trebuchet = new Trebuchet();

var day1Part1Result = await trebuchet.CalibrateAndSumAllAsync(xs, Trebuchet.Token.JUST_SINGLE_DIGITS);

Console.WriteLine($"Expected Results 54338, Actual Result {day1Part1Result.Value}");
Console.WriteLine();

Console.WriteLine("Running 2023 - Day 1 - Part 2 : Trebuchet");

xs = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

var day1Part2Result = await trebuchet.CalibrateAndSumAllAsync(xs, Trebuchet.Token.DIGITS_AND_WORDS);
Console.WriteLine($"Expected Results 53389, Actual Result {day1Part2Result.Value}");
Console.ReadLine();
