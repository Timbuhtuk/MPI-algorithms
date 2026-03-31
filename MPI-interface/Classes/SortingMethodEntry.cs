namespace MPI_interface;

public partial class MainWindow
{
    private sealed class SortingMethodEntry
    {
        public string Name { get; }
        public Action<int[]> Action { get; }
        public SortingMethodEntry(string name, Action<int[]> action)
        {
            Name = name;
            Action = action;
        }
    }
}