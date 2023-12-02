using AdventOfCode._2023.Day_1;

using var cts = new CancellationTokenSource(delay: TimeSpan.FromMinutes(5));

Console.WriteLine("Running 2023 - Day 1 - Part 1 : Trebuchet");

var toBeCalibrated = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

var day1Part1Result = await Trebuchet.CalibrateUsingDigitsAsync(toBeCalibrated).SumAsync();

Console.WriteLine($"Expected Results 54338, Actual Result {day1Part1Result}");
Console.WriteLine();

Console.WriteLine("Running 2023 - Day 1 - Part 2 : Trebuchet");

toBeCalibrated = File.ReadLinesAsync("2023/Day 1/Calibration-Input-File.txt", cts.Token);

var day1Part2Result = await Trebuchet.CalibrateUsingDigitsAndWordsAsync(toBeCalibrated).SumAsync();
Console.WriteLine($"Expected Results 53389, Actual Result {day1Part2Result}");
Console.ReadLine();
