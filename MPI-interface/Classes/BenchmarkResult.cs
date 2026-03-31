namespace MPI_interface.Classes
{
    public sealed class BenchmarkResult
    {
        public string MethodName { get; init; } = string.Empty;
        public int ArrayLength { get; init; }
        public long ElapsedTicks { get; init; }
        public double ElapsedMilliseconds { get; init; }
        public long AllocatedBytes { get; init; }
        public string DisplayTime
        {
            get
            {
                if (ElapsedMilliseconds < 0.0001)
                    return $"{ElapsedTicks:N0} ticks";
                else if (ElapsedMilliseconds < 0.005)
                    return $"{ElapsedMilliseconds:F4} ms";
                else if (ElapsedMilliseconds < 1)
                    return $"{ElapsedMilliseconds:F2} ms";
                else
                    return $"{ElapsedMilliseconds:F0} ms";
            }
        }
        public override string ToString()
        {
            return $"{MethodName}: " +
                   $"Length={ArrayLength}, " +
                   $"Time={ElapsedMilliseconds:F4} ms, " +
                   $"Ticks={ElapsedTicks}, " +
                   $"Allocated={AllocatedBytes} bytes";
        }
    }
}
