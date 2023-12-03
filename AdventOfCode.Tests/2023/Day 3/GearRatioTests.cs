using AdventOfCode._2023.Day_3.GearRatio;

namespace AdventOfCode.Tests._2023.Day_3
{
    [TestClass]
    public sealed class GearRatioTests
    {
        [TestMethod]
        public void EngineParts_ExtractedCorrectly()
        {   
            var rawEngineSchematic = new[] {
                "467..114..",
                "...*......",
                "..35..633.",
                "......#...",
                "617*......",
                ".....+.58.",
                "..592.....",
                "......755.",
                "...$.*....",
                ".664.598..",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(4361, engine.Parts.Sum(p => p.PartNumber));
        }

        [TestMethod]
        public async Task EngineParts_ExtractedCorrectlyFromSchematicDocument() 
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawEngineSchematic = await File.ReadAllLinesAsync("2023/Day 3/Engine-Schematic-File.txt", cts.Token);

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(539590, engine.Parts.Sum(p => p.PartNumber)); 
        }

        [TestMethod]
        public void EngineParts_ExtractedCorrectlyFromDiagonalAdjacentSymbol()
        {
            var rawEngineSchematic = new[] {
                "...*......",
                "....111...",
                "..........",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(111, engine.Parts.Sum(p => p.PartNumber));

            rawEngineSchematic = new[] {
                ".......*..",
                "....111...",
                "..........",
            };

            engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(111, engine.Parts.Sum(p => p.PartNumber));

            rawEngineSchematic = new[] {
                "..........",
                "....111...",
                "...*......",
            };

            engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(111, engine.Parts.Sum(p => p.PartNumber));

            rawEngineSchematic = new[] {
                "..........",
                "....111...",
                ".......*..",
            };

            engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(111, engine.Parts.Sum(p => p.PartNumber));
        }

        [TestMethod]
        public void EngineParts_ExtractedCorrectlyFromLeftAdjacentSymbol()
        {
            var rawEngineSchematic = new[] {
                "......*100",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(100, engine.Parts.Sum(p => p.PartNumber));
        }

        [TestMethod]
        public void EngineParts_ExtractedCorrectlyFromRightAdjacentSymbol()
        {
            var rawEngineSchematic = new[] {
                "......100*",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(100, engine.Parts.Sum(p => p.PartNumber));
        }

        [TestMethod]
        public void EngineParts_ExtractedCorrectlyFromAboveAdjacentSymbol()
        {
            var rawEngineSchematic = new[] {
                "........*.",
                ".......100",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(100, engine.Parts.Sum(p => p.PartNumber));
        }

        [TestMethod]
        public void EngineParts_ExtractedCorrectlyFromBelowAdjacentSymbol()
        {
            var rawEngineSchematic = new[] {
                ".......100",
                "........*.",
            };

            var engine = Engine.FromSchematic(rawEngineSchematic);

            Assert.AreEqual(100, engine.Parts.Sum(p => p.PartNumber));
        }
    }
}
