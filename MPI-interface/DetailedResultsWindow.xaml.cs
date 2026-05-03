using MPI_interface.Classes;
using MPI_interface.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MPI_interface;

public partial class DetailedResultsWindow : Window
{
    public DetailedResultsWindow(string algorithmName, IReadOnlyList<DetailedBenchmarkPoint> points)
    {
        InitializeComponent();
        HeaderTitle.Text = $"{algorithmName} — time vs array size";
        Loaded += (_, _) => BuildPlot(algorithmName, points);
    }

    private void BuildPlot(string algorithmName, IReadOnlyList<DetailedBenchmarkPoint> points)
    {
        if (points == null || points.Count == 0)
            return;

        MainPlot.Plot.Clear();
        MainPlot.Plot.Title($"{algorithmName} — average time");
        MainPlot.Plot.XLabel("Array size (elements)");
        MainPlot.Plot.YLabel("Time (ms)");

        try
        {
            MainPlot.Plot.FigureBackground.Color = ScottPlot.Colors.Black;
            MainPlot.Plot.DataBackground.Color = ScottPlot.Colors.Black;
            MainPlot.Plot.Axes.Color(ScottPlot.Colors.White);
        }
        catch
        {
        }

        (ArrayType type, string label, ScottPlot.Color color)[] series =
        {
            (ArrayType.Random, "Random", ScottPlot.Colors.Blue),
            (ArrayType.Reversed, "Reversed", ScottPlot.Colors.Red),
            (ArrayType.AlmostSorted, "Almost sorted", ScottPlot.Colors.Green)
        };

        double yMax = 0;
        foreach (var (type, label, color) in series)
        {
            var slice = points
                .Where(p => p.ArrayType == type)
                .OrderBy(p => p.ArrayLength)
                .ToList();
            if (slice.Count == 0)
                continue;

            double[] xs = slice.Select(p => (double)p.ArrayLength).ToArray();
            double[] ys = slice.Select(p => p.AvgElapsedMilliseconds).ToArray();
            yMax = Math.Max(yMax, ys.DefaultIfEmpty(0).Max());

            var scatter = MainPlot.Plot.Add.Scatter(xs, ys);
            scatter.LegendText = label;
            scatter.LineWidth = 2;
            scatter.MarkerSize = 7;
            scatter.Color = color;
        }

        MainPlot.Plot.ShowLegend();

        if (yMax > 0)
            MainPlot.Plot.Axes.SetLimits(bottom: 0, top: yMax * 1.15);

        MainPlot.Refresh();
    }
}
