using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MPI_interface.Classes;

public class BenchmarkTaskInfo : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private int _elementsCount;
    private int _arraysCount;
    private string _status = "Pending";
    private double _progressPercent;
    private bool _canOpenResults;
    private DateTime _createdAt = DateTime.Now;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public int ElementsCount
    {
        get => _elementsCount;
        set => SetField(ref _elementsCount, value);
    }

    public int ArraysCount
    {
        get => _arraysCount;
        set => SetField(ref _arraysCount, value);
    }

    public string Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }

    public double ProgressPercent
    {
        get => _progressPercent;
        set => SetField(ref _progressPercent, value);
    }

    public bool CanOpenResults
    {
        get => _canOpenResults;
        set => SetField(ref _canOpenResults, value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetField(ref _createdAt, value);
    }

    public List<BenchmarkResult> Results { get; set; } = new();

    public string Subtitle => $"Elements: {ElementsCount} | Arrays: {ArraysCount}";
    public string ProgressText => $"{ProgressPercent:F0}%";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        if (propertyName == nameof(ProgressPercent))
            OnPropertyChanged(nameof(ProgressText));

        if (propertyName is (nameof(ElementsCount)) or (nameof(ArraysCount)))
            OnPropertyChanged(nameof(Subtitle));

        return true;
    }
}