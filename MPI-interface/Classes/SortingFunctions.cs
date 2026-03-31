using System.Runtime.InteropServices;

public static class SortingFunctions
{
    private const string DLL_NAME = "C:\\Users\\timpf\\source\\repos\\MPI-algorithms\\x64\\Debug\\MPI-algorithms.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void quick_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void merge_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void heap_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void timsort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bubble_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void insertion_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern void bogo_sort(int[] arr, int len);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    private static extern int stalin_sort(int[] arr, int len);

    public static void QuickSort(int[] arr)
    {
        ValidateArray(arr);
        quick_sort(arr, arr.Length);
    }

    public static void QuickSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        quick_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void MergeSort(int[] arr)
    {
        ValidateArray(arr);
        merge_sort(arr, arr.Length);
    }

    public static void MergeSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        merge_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void HeapSort(int[] arr)
    {
        ValidateArray(arr);
        heap_sort(arr, arr.Length);
    }

    public static void HeapSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        heap_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void TimSort(int[] arr)
    {
        ValidateArray(arr);
        timsort(arr, arr.Length);
    }

    public static void TimSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        timsort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void bubble_sort(int[] arr)
    {
        ValidateArray(arr);
        bubble_sort(arr, arr.Length);
    }

    public static void bubble_sort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        bubble_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void InsertionSort(int[] arr)
    {
        ValidateArray(arr);
        insertion_sort(arr, arr.Length);
    }

    public static void InsertionSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        insertion_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void BogoSort(int[] arr)
    {
        ValidateArray(arr);
        bogo_sort(arr, arr.Length);
    }

    public static void BogoSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        bogo_sort(arr, arr.Length);
        CopyToList(arr, list);
    }

    public static void StalinSort(int[] arr)
    {
        ValidateArray(arr);
        int newLen = stalin_sort(arr, arr.Length);

        int[] result = new int[newLen];
        Array.Copy(arr, result, newLen);
    }

    public static List<int> StalinSort(List<int> list)
    {
        ValidateList(list);
        int[] arr = list.ToArray();
        int newLen = stalin_sort(arr, arr.Length);

        List<int> result = new(newLen);
        for (int i = 0; i < newLen; i++)
            result.Add(arr[i]);

        return result;
    }

    private static void ValidateArray(int[] arr)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));
    }

    private static void ValidateList(List<int> list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
    }

    private static void CopyToList(int[] source, List<int> target)
    {
        target.Clear();
        target.AddRange(source);
    }
}