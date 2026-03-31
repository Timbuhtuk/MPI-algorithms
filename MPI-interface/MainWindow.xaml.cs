using MPI_interface.Classes;
using MPI_interface.Enums;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MPI_interface;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public ObservableCollection<BenchmarkTaskInfo> Tasks { get; } = new();
    private ArrayType _selectedArrayType;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ArrayType SelectedArrayType
    {
        get => _selectedArrayType;
        set
        {
            if (_selectedArrayType != value)
            {
                _selectedArrayType = value;
                NotifyPropertyChanged(nameof(SelectedArrayType));
            }
        }
    }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e) { }

    private async void StartBenchmarkButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadSettings(out int elementsCount, out int arraysCount))
            return;

        List<SortingMethodEntry> methods = GetSelectedMethods();

        BenchmarkTaskInfo task = new()
        {
            Title = $"Benchmark #{Tasks.Count + 1}",
            ElementsCount = elementsCount,
            ArraysCount = arraysCount,
            Status = "Running",
            ProgressPercent = 0,
            CanOpenResults = false,
            CreatedAt = DateTime.Now
        };

        Tasks.Insert(0, task);

        try
        {
            List<BenchmarkResult> results = await RunBenchmarksAsync(
                task,
                elementsCount,
                arraysCount,
                methods);

            task.Results = results;
            task.ProgressPercent = 100;
            task.Status = "Completed";
            task.CanOpenResults = true;
        }
        catch (Exception ex)
        {
            task.Status = $"Failed: {ex.Message}";
            task.CanOpenResults = false;
        }
    }

    private bool TryReadSettings(out int elementsCount, out int arraysCount)
    {
        elementsCount = 0;
        arraysCount = 0;

        if (!int.TryParse(ElementsCountTextBox.Text, out elementsCount) || elementsCount <= 0)
        {
            MessageBox.Show("Введите корректное количество элементов.");
            return false;
        }

        if (!int.TryParse(ArraysCountTextBox.Text, out arraysCount) || arraysCount <= 0)
        {
            MessageBox.Show("Введите корректное количество массивов.");
            return false;
        }

        if (!GetSelectedMethods().Any())
        {
            MessageBox.Show("Выберите хотя бы один метод сортировки.");
            return false;
        }

        return true;
    }

    private List<SortingMethodEntry> GetSelectedMethods()
    {
        List<SortingMethodEntry> methods = new();

        if (QuickSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("QuickSort", SortingFunctions.QuickSort));

        if (MergeSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("MergeSort", SortingFunctions.MergeSort));

        if (HeapSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("HeapSort", SortingFunctions.HeapSort));

        if (TimSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("TimSort", SortingFunctions.TimSort));

        if (BubbleSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("BubbleSort", SortingFunctions.bubble_sort));

        if (InsertionSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("InsertionSort", SortingFunctions.InsertionSort));

        if (BogoSortCheckBox.IsChecked == true)
            methods.Add(new SortingMethodEntry("BogoSort", SortingFunctions.BogoSort));

        return methods;
    }

    private async Task<List<BenchmarkResult>> RunBenchmarksAsync(
        BenchmarkTaskInfo task,
        int elementsCount,
        int arraysCount,
        List<SortingMethodEntry> methods)
    {
        return await Task.Run(() =>
        {
            List<BenchmarkResult> results = new();
            int total = methods.Count;
            int completed = 0;

            foreach (SortingMethodEntry method in methods)
            {
                int maxValue = 100_000;
                int runs = arraysCount;

                int[] sample = new int[elementsCount].FillRandom(0, maxValue);
                BenchmarkResult result = RunAverageBenchmark(method.Name, method.Action, sample, runs);
                results.Add(result);

                completed++;
                double progress = total == 0 ? 0 : (double)completed / total * 100.0;

                Dispatcher.Invoke(() =>
                {
                    task.ProgressPercent = progress;
                    task.Status = $"Running ({completed}/{total})";
                });
            }

            return results;
        });
    }

    private static BenchmarkResult RunAverageBenchmark(
        string displayName,
        Action<int[]> sortingMethod,
        int[] sourceArray,
        int runs)
    {
        double totalMs = 0;
        long totalTicks = 0;
        long totalBytes = 0;

        for (int i = 0; i < runs; i++)
        {
            int[] copy = (int[])sourceArray.Clone();
            BenchmarkResult result = Benchmark.Bench(sortingMethod, copy);

            totalMs += result.ElapsedMilliseconds;
            totalTicks += result.ElapsedTicks;
            totalBytes += result.AllocatedBytes;
        }

        return new BenchmarkResult
        {
            MethodName = displayName,
            ElapsedMilliseconds = totalMs / runs,
            ElapsedTicks = totalTicks / runs,
            AllocatedBytes = totalBytes / runs
        };
    }
}