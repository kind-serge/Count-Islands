using System;
using System.Diagnostics;
using System.Text;

namespace Islands
{
    class Program
    {
        unsafe static void Main(string[] args)
        {
            var w = 10_000;
            var h = 10_000;

            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine($"Generating fractal map {w:N0} x {h:N0}...");
            var data = PlasmaFractalGenerator.NewIslandMap(w, h, seed: Environment.TickCount);

            TimeSpan baseTime;

            // regular graph traversal
            {
                fixed (int* pData = data)
                {
                    var sw = Stopwatch.StartNew();
                    var n = GraphTraversal.CountIslands(pData, w, h);
                    baseTime = sw.Elapsed;

                    if (w < Console.WindowWidth / 2 && h < Console.WindowWidth / 2)
                        RenderIslandMap(pData, w, h);

                    Console.WriteLine();
                    Console.WriteLine("Graph Traversal");
                    Console.WriteLine($"  time: {baseTime}");
                    Console.WriteLine($"  islands: {n}");

                    Reset(pData, w, h);
                }
            }

            // reconciliation algorithm
            {
                fixed (int* pData = data)
                {
                    var sw = Stopwatch.StartNew();
                    var n = Reconciliation.CountIslands(pData, w, h);
                    var time = sw.Elapsed;

                    Console.WriteLine();
                    Console.WriteLine("Reconciliation");
                    Console.WriteLine($"  time: {time}");
                    Console.WriteLine($"  islands: {n}");
                    Console.WriteLine($"  speedup: {baseTime.TotalMilliseconds / time.TotalMilliseconds:N1}X");

                    Reset(pData, w, h);
                }
            }

            // parallel reconciliation algorithm
            {
                fixed (int* pData = data)
                {
                    var dop = Environment.ProcessorCount;
                    var sw = Stopwatch.StartNew();
                    var n = ParallelReconciliation.CountIslands(pData, w, h, threadCount: dop);
                    var time = sw.Elapsed;

                    Console.WriteLine();
                    Console.WriteLine($"Parallel Reconciliation (DoP: {dop})");
                    Console.WriteLine($"  time: {time}");
                    Console.WriteLine($"  islands: {n}");
                    Console.WriteLine($"  speedup: {baseTime.TotalMilliseconds / time.TotalMilliseconds:N1}X");
                }
            }

            Console.WriteLine();
        }

        static unsafe void Reset(int* pData, int w, int h)
        {
            var end = pData + w * h;
            for (; pData != end; pData++)
                if (*pData != 0)
                    *pData = 1;
        }

        unsafe static void RenderIslandMap(int* pData, int w, int h)
        {
            var origColor = Console.ForegroundColor;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var v = pData[x + y * w];
                    if (v == 0)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.ForegroundColor = (ConsoleColor)((v % 15) + 1);
                        Console.Write("██");
                    }
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = origColor;
        }
    }
}
