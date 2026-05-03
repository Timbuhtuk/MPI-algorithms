using MPI_interface.Enums;

namespace MPI_interface.Classes;

public sealed class DetailedBenchmarkPoint
{
    public int ArrayLength { get; init; }
    public ArrayType ArrayType { get; init; }
    public double AvgElapsedMilliseconds { get; init; }
}
