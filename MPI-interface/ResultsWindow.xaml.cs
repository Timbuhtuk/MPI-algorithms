using MPI_interface.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MPI_interface;

public partial class ResultsWindow : Window
{
    private readonly List<BenchmarkResult> _results;

    public ResultsWindow(List<BenchmarkResult> results)
    {
        InitializeComponent();
        _results = (results ?? new List<BenchmarkResult>())
            .Where(x => x != null)
            .OrderBy(x => x.MethodName)
            .ToList();
        Loaded += ResultsWindow_Loaded;
    }

    private void ResultsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            FillLists();
            BuildPlots();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Не удалось отобразить результаты.\n\n{ex.GetType().Name}: {ex.Message}",
                "Ошибка построения результатов",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void FillLists()
    {
        TimeResultsListBox.ItemsSource = _results
            .OrderBy(x => x.ElapsedMilliseconds)
            .ToList();
    }

    private void BuildPlots()
    {
        BuildTimePlot();
    }

    private void BuildTimePlot()
    {
        if (_results.Count == 0)
            return;

        double[] positions = Enumerable.Range(0, _results.Count).Select(x => (double)x).ToArray();
        double[] values = _results.Select(x => x.ElapsedMilliseconds).ToArray();
        string[] labels = _results.Select(x => x.MethodName.Replace("Sort", "")).ToArray();

        PlotTop.Plot.Clear();
        ConfigurePlotAppearance(PlotTop, "Execution Time", "ms");

        PlotTop.Plot.Axes.SetLimits(left: -0.5, right: _results.Count - 0.5, bottom: 0, top: values.Max() + 10);

        var bars = PlotTop.Plot.Add.Bars(positions, values);
        bars.ValueLabelStyle.IsVisible = true;

        PlotTop.Plot.Axes.Bottom.TickGenerator =
            new ScottPlot.TickGenerators.NumericManual(positions, labels);

        PlotTop.Refresh();
    }

    private static void ConfigurePlotAppearance(ScottPlot.WPF.WpfPlot plot, string title, string yLabel)
    {
        plot.Plot.Clear();
        plot.Plot.Title(title);
        plot.Plot.XLabel("Algorithm");
        plot.Plot.YLabel(yLabel);

        try
        {
            plot.Plot.FigureBackground.Color = ScottPlot.Colors.Black;
            plot.Plot.DataBackground.Color = ScottPlot.Colors.Black;
            plot.Plot.Axes.Color(ScottPlot.Colors.White);
        }
        catch
        {
        }

        plot.Refresh();
    }
}
