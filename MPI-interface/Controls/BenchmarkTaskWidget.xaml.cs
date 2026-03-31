using MPI_interface.Classes;
using System.Windows;
using System.Windows.Controls;

namespace MPI_interface.Controls;

public partial class BenchmarkTaskWidget : UserControl
{
    public BenchmarkTaskWidget()
    {
        InitializeComponent();
    }

    private void OpenResults_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not BenchmarkTaskInfo task)
            return;

        if (task.Results == null || task.Results.Count == 0)
            return;

        ResultsWindow resultsWindow = new(task.Results)
        {
            Owner = Window.GetWindow(this)
        };

        resultsWindow.Show();
        resultsWindow.Activate();
    }
}