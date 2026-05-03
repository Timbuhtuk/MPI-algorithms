using MPI_interface.Classes;
using MPI_interface.Enums;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MPI_interface;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public ObservableCollection<BenchmarkTaskInfo> Tasks { get; } = new();
    private ArrayType _selectedArrayType;
    private bool _isNarrowLayout;

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

    public bool IsNarrowLayout
    {
        get => _isNarrowLayout;
        private set
        {
            if (_isNarrowLayout != value)
            {
                _isNarrowLayout = value;
                NotifyPropertyChanged(nameof(IsNarrowLayout));
            }
        }
    }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += MainWindow_Loaded;
        SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateSettingsLayout();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Switch layout when the left side would become cramped.
        IsNarrowLayout = ActualWidth < 900;
        UpdateSettingsLayout();
    }

    private void UpdateSettingsLayout()
    {
        bool narrow = IsNarrowLayout;

        // Wide (default): Elements + Arrays in 2 columns, then Seed full width, then ArrayType row.
        if (!narrow)
        {
            Grid.SetRow(ElementsCard, 0);
            Grid.SetColumn(ElementsCard, 0);
            Grid.SetColumnSpan(ElementsCard, 1);

            Grid.SetRow(ArraysCard, 0);
            Grid.SetColumn(ArraysCard, 2);
            Grid.SetColumnSpan(ArraysCard, 1);
            ArraysCard.Margin = new Thickness(0);

            Grid.SetRow(SeedCard, 1);
            Grid.SetColumn(SeedCard, 0);
            Grid.SetColumnSpan(SeedCard, 3);
            SeedCard.Margin = new Thickness(0, 12, 0, 0);

            Grid.SetRow(ArrayTypeRow, 2);
            Grid.SetColumn(ArrayTypeRow, 0);
            Grid.SetColumnSpan(ArrayTypeRow, 3);
            ArrayTypeRow.Margin = new Thickness(0, 12, 0, 0);

            return;
        }

        // Narrow: stack all cards full-width.
        Grid.SetRow(ElementsCard, 0);
        Grid.SetColumn(ElementsCard, 0);
        Grid.SetColumnSpan(ElementsCard, 3);

        Grid.SetRow(ArraysCard, 1);
        Grid.SetColumn(ArraysCard, 0);
        Grid.SetColumnSpan(ArraysCard, 3);
        ArraysCard.Margin = new Thickness(0, 12, 0, 0);

        Grid.SetRow(SeedCard, 2);
        Grid.SetColumn(SeedCard, 0);
        Grid.SetColumnSpan(SeedCard, 3);
        SeedCard.Margin = new Thickness(0, 12, 0, 0);

        Grid.SetRow(ArrayTypeRow, 3);
        Grid.SetColumn(ArrayTypeRow, 0);
        Grid.SetColumnSpan(ArrayTypeRow, 3);
        ArrayTypeRow.Margin = new Thickness(0, 12, 0, 0);
    }

    private async void StartBenchmarkButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadSettings(out int elementsCount, out int arraysCount, out int? seed))
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
                methods,
                seed,
                SelectedArrayType);

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

    private bool TryReadSettings(out int elementsCount, out int arraysCount, out int? seed)
    {
        elementsCount = 0;
        arraysCount = 0;
        seed = null;

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

        string seedText = SeedTextBox.Text?.Trim() ?? "";
        if (!string.IsNullOrWhiteSpace(seedText))
        {
            if (!int.TryParse(seedText, out int parsedSeed))
            {
                MessageBox.Show("Введите корректный seed (целое число) или оставьте поле пустым.");
                return false;
            }
            seed = parsedSeed;
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
        List<SortingMethodEntry> methods,
        int? seed,
        ArrayType arrayType)
    {
        return await Task.Run(() =>
        {
            List<BenchmarkResult> results = new();
            int total = methods.Count;
            int completed = 0;
            Random? rng = seed.HasValue ? new Random(seed.Value) : null;

            foreach (SortingMethodEntry method in methods)
            {
                int maxValue = 100_000;
                int runs = arraysCount;

                int[] sample = GenerateSample(elementsCount, maxValue, arrayType, rng);
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

    private static int[] GenerateSample(int elementsCount, int maxValue, ArrayType arrayType, Random? rng)
    {
        int[] sample = new int[elementsCount];

        return arrayType switch
        {
            ArrayType.Random => rng is null
                ? sample.FillRandom(0, maxValue)
                : sample.FillRandom(rng, 0, maxValue),
            ArrayType.Reversed => sample.FillReversed(),
            ArrayType.AlmostSorted => rng is null
                ? sample.FillAlmostSorted(swaps: Math.Max(1, elementsCount / 1000))
                : sample.FillAlmostSorted(rng, swaps: Math.Max(1, elementsCount / 1000)),
            _ => rng is null
                ? sample.FillRandom(0, maxValue)
                : sample.FillRandom(rng, 0, maxValue)
        };
    }

    private static BenchmarkResult RunAverageBenchmark(
        string displayName,
        Action<int[]> sortingMethod,
        int[] sourceArray,
        int runs)
    {
        double totalMs = 0;
        long totalTicks = 0;

        for (int i = 0; i < runs; i++)
        {
            int[] copy = (int[])sourceArray.Clone();
            BenchmarkResult result = Benchmark.Bench(sortingMethod, copy);

            totalMs += result.ElapsedMilliseconds;
            totalTicks += result.ElapsedTicks;
        }

        return new BenchmarkResult
        {
            MethodName = displayName,
            ElapsedMilliseconds = totalMs / runs,
            ElapsedTicks = totalTicks / runs
        };
    }

    private async void DetailedRunButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadDetailedSettings(
                out int startSize,
                out int maxSize,
                out double multiplier,
                out int runsPerSize,
                out int? seed))
            return;

        if (!TryGetDetailedAlgorithm(out string algoName, out Action<int[]>? sortAction) || sortAction is null)
        {
            MessageBox.Show("Выберите алгоритм.");
            return;
        }

        List<int> sizes = BuildSizes(startSize, maxSize, multiplier);
        if (sizes.Count == 0)
        {
            MessageBox.Show("Некорректный диапазон размеров (Start / Max / Multiplier).");
            return;
        }

        long totalSteps = (long)sizes.Count * 3 * runsPerSize;
        if (totalSteps <= 0)
            return;

        DetailedRunButton.IsEnabled = false;
        DetailedProgressBar.Value = 0;
        DetailedStatusTextBlock.Text = "Starting…";

        try
        {
            List<DetailedBenchmarkPoint> points = await Task.Run(() =>
                RunDetailedSweep(algoName, sortAction, sizes, runsPerSize, seed, totalSteps));

            var win = new DetailedResultsWindow(algoName, points)
            {
                Owner = this
            };
            win.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Детальный тест не выполнен.\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            DetailedRunButton.IsEnabled = true;
            DetailedStatusTextBlock.Text = "Idle";
            DetailedProgressBar.Value = 0;
        }
    }

    private bool TryReadDetailedSettings(
        out int startSize,
        out int maxSize,
        out double multiplier,
        out int runsPerSize,
        out int? seed)
    {
        startSize = 0;
        maxSize = 0;
        multiplier = 0;
        runsPerSize = 0;
        seed = null;

        if (!int.TryParse(DetailedStartSizeTextBox.Text, out startSize) || startSize < 1)
        {
            MessageBox.Show("Start size: целое число ≥ 1.");
            return false;
        }

        if (!int.TryParse(DetailedMaxSizeTextBox.Text, out maxSize) || maxSize < startSize)
        {
            MessageBox.Show("Max size (N): целое число ≥ Start size.");
            return false;
        }

        if (!double.TryParse(DetailedMultiplierTextBox.Text.Replace(',', '.'), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out multiplier) || multiplier <= 1.0)
        {
            MessageBox.Show("Multiplier: число > 1 (например 2).");
            return false;
        }

        long probeNext = (long)Math.Floor(startSize * multiplier);
        if (startSize < maxSize && probeNext <= startSize)
        {
            MessageBox.Show("Multiplier слишком мал: следующий размер не превышает текущий.");
            return false;
        }

        if (!int.TryParse(DetailedRunsPerSizeTextBox.Text, out runsPerSize) || runsPerSize < 1)
        {
            MessageBox.Show("Arrays per size: целое число ≥ 1.");
            return false;
        }

        string seedText = DetailedSeedTextBox.Text?.Trim() ?? "";
        if (!string.IsNullOrWhiteSpace(seedText))
        {
            if (!int.TryParse(seedText, out int parsedSeed))
            {
                MessageBox.Show("Seed: целое число или пусто.");
                return false;
            }

            seed = parsedSeed;
        }

        return true;
    }

    private bool TryGetDetailedAlgorithm(out string name, out Action<int[]>? action)
    {
        name = "";
        action = null;

        if (DetailedQuickRadio.IsChecked == true)
        {
            name = "QuickSort";
            action = SortingFunctions.QuickSort;
            return true;
        }

        if (DetailedMergeRadio.IsChecked == true)
        {
            name = "MergeSort";
            action = SortingFunctions.MergeSort;
            return true;
        }

        if (DetailedHeapRadio.IsChecked == true)
        {
            name = "HeapSort";
            action = SortingFunctions.HeapSort;
            return true;
        }

        if (DetailedTimRadio.IsChecked == true)
        {
            name = "TimSort";
            action = SortingFunctions.TimSort;
            return true;
        }

        if (DetailedBubbleRadio.IsChecked == true)
        {
            name = "BubbleSort";
            action = SortingFunctions.bubble_sort;
            return true;
        }

        if (DetailedInsertionRadio.IsChecked == true)
        {
            name = "InsertionSort";
            action = SortingFunctions.InsertionSort;
            return true;
        }

        if (DetailedBogoRadio.IsChecked == true)
        {
            name = "BogoSort";
            action = SortingFunctions.BogoSort;
            return true;
        }

        if (DetailedStalinRadio.IsChecked == true)
        {
            name = "StalinSort";
            action = static arr => SortingFunctions.StalinSort(arr);
            return true;
        }

        return false;
    }

    private static List<int> BuildSizes(int startSize, int maxSize, double multiplier)
    {
        List<int> sizes = new();
        long cur = startSize;

        while (cur <= maxSize)
        {
            sizes.Add((int)cur);
            long next = (long)Math.Floor(cur * multiplier);
            if (next <= cur)
                break;
            cur = next;
        }

        return sizes;
    }

    private List<DetailedBenchmarkPoint> RunDetailedSweep(
        string algoName,
        Action<int[]> sortAction,
        List<int> sizes,
        int runsPerSize,
        int? seed,
        long totalSteps)
    {
        Random? rng = seed.HasValue ? new Random(seed.Value) : null;
        int maxValue = 100_000;
        ArrayType[] types =
        [
            ArrayType.Random,
            ArrayType.Reversed,
            ArrayType.AlmostSorted
        ];

        List<DetailedBenchmarkPoint> points = new();
        long done = 0;

        foreach (int size in sizes)
        {
            foreach (ArrayType arrayType in types)
            {
                double sumMs = 0;

                for (int r = 0; r < runsPerSize; r++)
                {
                    int[] sample = GenerateSample(size, maxValue, arrayType, rng);
                    BenchmarkResult bench = Benchmark.Bench(sortAction, sample);
                    sumMs += bench.ElapsedMilliseconds;

                    done++;
                    double pct = totalSteps == 0 ? 0 : (double)done / totalSteps * 100.0;
                    Dispatcher.Invoke(() =>
                    {
                        DetailedProgressBar.Value = pct;
                        DetailedStatusTextBlock.Text =
                            $"{algoName}: size {size}, {arrayType}, run {r + 1}/{runsPerSize}";
                    });
                }

                points.Add(new DetailedBenchmarkPoint
                {
                    ArrayLength = size,
                    ArrayType = arrayType,
                    AvgElapsedMilliseconds = sumMs / runsPerSize
                });
            }
        }

        return points;
    }
}