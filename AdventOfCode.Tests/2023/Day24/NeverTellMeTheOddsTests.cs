using AdventOfCode._2023.Day24;
using Microsoft.Z3;
using System.Xml.Linq;
using Z3.Linq;

namespace AdventOfCode.Tests._2023.Day24
{
    [TestClass]
    public sealed class NeverTellMeTheOddsTests
    {
        [TestMethod]
        public void CountOfFutureHailstoneCollisions_CalculatedCorrectly()
        {
            const long MIN_BOUNDED = 7;
            const long MAX_BOUNDED = 27;

            var rawHailstonesMap = "19, 13, 30 @ -2,  1, -2\r\n18, 19, 22 @ -1, -1, -2\r\n20, 25, 34 @ -2, -2, -4\r\n12, 31, 28 @ -1, -2, -1\r\n20, 19, 15 @  1, -5, -3".Split(System.Environment.NewLine);

            var hailstones = Hailstones.Parse(rawHailstonesMap);

            var countOfCollisions = hailstones.CountOfFutureHailstoneCollisions((MIN_BOUNDED, MAX_BOUNDED));

            Assert.AreEqual(2, countOfCollisions);
        }

        [TestMethod]
        public async Task CountOfFutureHailstoneCollisions_CalculatedCorrectly_FromFile()
        {
            const long MIN_BOUNDED = 200000000000000;
            const long MAX_BOUNDED = 400000000000000;

            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHailstonesMap = await File.ReadAllLinesAsync("2023/Day24/Never-Tell-Me-The-Odds-File.txt", cts.Token);

            var hailstones = Hailstones.Parse(rawHailstonesMap);

            var countOfCollisions = hailstones.CountOfFutureHailstoneCollisions((MIN_BOUNDED, MAX_BOUNDED));

            Assert.AreEqual(20847, countOfCollisions);
        }

        [TestMethod]
        public void InitialRockPositionAndVelocity_ToHitAllHailstones_CorrectlyCalculated_UsingZ3Solver()
        {
            var rawHailstonesMap = "19, 13, 30 @ -2,  1, -2\r\n18, 19, 22 @ -1, -1, -2\r\n20, 25, 34 @ -2, -2, -4\r\n12, 31, 28 @ -1, -2, -1\r\n20, 19, 15 @  1, -5, -3".Split(System.Environment.NewLine);

            var hailstones = Hailstones.Parse(rawHailstonesMap);

            using var ctxt = new Context();

            using var solver = ctxt.MkSolver();

            // Define some placeholders for the velocity and position values our rock could start with

            using var x = ctxt.MkIntConst("x");
            using var y = ctxt.MkIntConst("y");
            using var z = ctxt.MkIntConst("z");

            using var vx = ctxt.MkIntConst("vx");
            using var vy = ctxt.MkIntConst("vy");
            using var vz = ctxt.MkIntConst("vz");

            // For each hailstone, describe a time line (t) for it (from 0 to infinity) and then construct the 
            // constraints of our puzzle (the equations we are trying to solve)

            foreach (var hailstone in hailstones)
            {
                using var t = ctxt.MkIntConst($"t{hailstone.Id}");

                solver.Add(t >= 0);     // the timeline of the hailstone should always be moving forward from 0

                using var px = ctxt.MkInt(Convert.ToInt64(hailstone.Position.X));
                using var py = ctxt.MkInt(Convert.ToInt64(hailstone.Position.Y));
                using var pz = ctxt.MkInt(Convert.ToInt64(hailstone.Position.Z));

                using var pvx = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.X));
                using var pvy = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.Y));
                using var pvz = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.Z));

                using var sx = ctxt.MkEq(ctxt.MkAdd(x, ctxt.MkMul(t, vx)), ctxt.MkAdd(px, ctxt.MkMul(t, pvx)));
                using var sy = ctxt.MkEq(ctxt.MkAdd(y, ctxt.MkMul(t, vy)), ctxt.MkAdd(py, ctxt.MkMul(t, pvy)));
                using var sz = ctxt.MkEq(ctxt.MkAdd(z, ctxt.MkMul(t, vz)), ctxt.MkAdd(pz, ctxt.MkMul(t, pvz)));

                solver.Add(sx);         // x + t * vx = px + t * pvx
                solver.Add(sy);         // y + t * vy = py + t * pvy
                solver.Add(sz);         // z + t * vz = pz + t * pvz
            }

            _ = solver.Check();

            using var model = solver.Model;

            using var rx = model.Eval(x);
            using var ry = model.Eval(y);
            using var rz = model.Eval(z);

            using var rvx = model.Eval(vx);
            using var rvy = model.Eval(vy);
            using var rvz = model.Eval(vz);

            var startingPosition = new Vector(long.Parse(rx.ToString()), long.Parse(ry.ToString()), long.Parse(rz.ToString()));
            var startingVelocity = new Vector(long.Parse(rvx.ToString()), long.Parse(rvy.ToString()), long.Parse(rvz.ToString()));

            Assert.AreEqual(new(24, 13, 10), startingPosition);
            Assert.AreEqual(new(-3, 1, 2), startingVelocity);

            var actualResult = startingPosition.X + startingPosition.Y + startingPosition.Z;

            Assert.AreEqual(47, actualResult);
        }

        [TestMethod]
        public async Task InitialRockPositionAndVelocity_ToHitAllHailstones_CorrectlyCalculated_UsingZ3Solver_FromFile()
        {
            using var cts = new CancellationTokenSource(delay: TimeSpan.FromSeconds(5));

            var rawHailstonesMap = await File.ReadAllLinesAsync("2023/Day24/Never-Tell-Me-The-Odds-File.txt", cts.Token);

            var hailstones = Hailstones.Parse(rawHailstonesMap);

            using var ctxt = new Context();

            using var solver = ctxt.MkSolver();

            // Define some placeholders for the velocity and position values our rock could start with

            using var x = ctxt.MkIntConst("x");
            using var y = ctxt.MkIntConst("y");
            using var z = ctxt.MkIntConst("z");

            using var vx = ctxt.MkIntConst("vx");
            using var vy = ctxt.MkIntConst("vy");
            using var vz = ctxt.MkIntConst("vz");

            // For each hailstone, describe a time line (t) for it (from 0 to infinity) and then construct the 
            // constraints of our puzzle (the equations we are trying to solve)

            foreach (var hailstone in hailstones)
            {
                using var t = ctxt.MkIntConst($"t{hailstone.Id}");

                solver.Add(t >= 0);     // the timeline of the hailstone should always be moving forward from 0

                using var px = ctxt.MkInt(Convert.ToInt64(hailstone.Position.X));
                using var py = ctxt.MkInt(Convert.ToInt64(hailstone.Position.Y));
                using var pz = ctxt.MkInt(Convert.ToInt64(hailstone.Position.Z));

                using var pvx = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.X));
                using var pvy = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.Y));
                using var pvz = ctxt.MkInt(Convert.ToInt64(hailstone.Velocity.Z));

                using var sx = ctxt.MkEq(ctxt.MkAdd(x, ctxt.MkMul(t, vx)), ctxt.MkAdd(px, ctxt.MkMul(t, pvx)));
                using var sy = ctxt.MkEq(ctxt.MkAdd(y, ctxt.MkMul(t, vy)), ctxt.MkAdd(py, ctxt.MkMul(t, pvy)));
                using var sz = ctxt.MkEq(ctxt.MkAdd(z, ctxt.MkMul(t, vz)), ctxt.MkAdd(pz, ctxt.MkMul(t, pvz)));

                solver.Add(sx);         // x + t * vx = px + t * pvx
                solver.Add(sy);         // y + t * vy = py + t * pvy
                solver.Add(sz);         // z + t * vz = pz + t * pvz
            }

            _ = solver.Check();

            using var model = solver.Model;

            using var rx = model.Eval(x);
            using var ry = model.Eval(y);
            using var rz = model.Eval(z);

            using var rvx = model.Eval(vx);
            using var rvy = model.Eval(vy);
            using var rvz = model.Eval(vz);

            var startingPosition = new Vector(long.Parse(rx.ToString()), long.Parse(ry.ToString()), long.Parse(rz.ToString()));
            var startingVelocity = new Vector(long.Parse(rvx.ToString()), long.Parse(rvy.ToString()), long.Parse(rvz.ToString()));

            var actualResult = startingPosition.X + startingPosition.Y + startingPosition.Z;

            Assert.AreEqual(908621716620524, actualResult);
        }


        //[TestMethod]
        public void InitialRockPositionAndVelocity_ToHitAllHailstones_CorrectlyCalculated_UsingZ3Solver_AndLinq()
        {
            var rawHailstonesMap = "19, 13, 30 @ -2,  1, -2\r\n18, 19, 22 @ -1, -1, -2\r\n20, 25, 34 @ -2, -2, -4\r\n12, 31, 28 @ -1, -2, -1\r\n20, 19, 15 @  1, -5, -3".Split(System.Environment.NewLine);

            var hailstones = Hailstones.Parse(rawHailstonesMap);

            using var ctx = new Z3Context();

            // Unfortunately, Z3.Linq doesn't support the query we are trying to build.  Baulks at the Convert.ToInt64 call, and then if we 
            // remove it we get the second exception. Also doesn't support the 'let' query syntax which would have made the query much more
            // readable :(
            //
            // System.NotSupportedException: 'Unsupported expression node type encountered: ConvertChecked'
            // System.InvalidCastException: 'Unable to cast object of type 'Microsoft.Z3.IntExpr' to type 'Microsoft.Z3.IntNum'.'

            var solver = from rock in ctx.NewTheorem<(Hailstone Hailstone, int Tick, (int X, int Y, int Z) Position, (int X, int Y, int Z) Velocity)>()
                         where rock.Tick >= 0
                         where rock.Position.X + rock.Tick * rock.Velocity.X == Convert.ToInt64(rock.Hailstone.Position.X) + rock.Tick * Convert.ToInt64(rock.Hailstone.Velocity.X)
                         where rock.Position.Y + rock.Tick * rock.Velocity.Y == Convert.ToInt64(rock.Hailstone.Position.Y) + rock.Tick * Convert.ToInt64(rock.Hailstone.Velocity.Y)
                         where rock.Position.Z + rock.Tick * rock.Velocity.Z == Convert.ToInt64(rock.Hailstone.Position.Z) + rock.Tick * Convert.ToInt64(rock.Hailstone.Velocity.Z)
                         select rock;

            var result = solver.Solve();

            var startingPosition = result.Position;
            var startingVelocity = result.Velocity;

            Assert.AreEqual(new(24, 13, 10), startingPosition);
            Assert.AreEqual(new(-3, 1, 2), startingVelocity);

            var actualResult = startingPosition.X + startingPosition.Y + startingPosition.Z;

            Assert.AreEqual(47, actualResult);
        }
    }
}
