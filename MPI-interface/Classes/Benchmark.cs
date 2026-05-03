using System.Diagnostics;

namespace MPI_interface.Classes;

public static class Benchmark
{
    public static BenchmarkResult Bench(Action<int[]> method, int[] source)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        if (source == null)
            throw new ArgumentNullException(nameof(source));

        int[] workingArray = (int[])source.Clone();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Stopwatch stopwatch = Stopwatch.StartNew();
        method(workingArray);
        stopwatch.Stop();

        return new BenchmarkResult
        {
            MethodName = method.Method.Name,
            ArrayLength = workingArray.Length,
            ElapsedTicks = stopwatch.ElapsedTicks,
            ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
        };
    }
}
