namespace MPI_interface.Classes;

public static class Support
{
    public static int[] FillRandom(this int[] arr, int min = 0, int max = int.MaxValue)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = Random.Shared.Next(min, max);
        }
        return arr;
    }

    public static int[] FillRandom(this int[] arr, Random rng, int min = 0, int max = int.MaxValue)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));
        if (rng == null)
            throw new ArgumentNullException(nameof(rng));

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = rng.Next(min, max);
        }
        return arr;
    }

    public static int[] FillAlmostSorted(this int[] arr, int swaps = 10)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));
        if (arr.Length == 0)
            return arr;

        for (int i = 0; i < arr.Length; i++)
            arr[i] = i;

        for (int i = 0; i < swaps; i++)
        {
            int a = Random.Shared.Next(0, arr.Length);
            int b = Random.Shared.Next(0, arr.Length);

            (arr[a], arr[b]) = (arr[b], arr[a]);
        }

        return arr;
    }

    public static int[] FillAlmostSorted(this int[] arr, Random rng, int swaps = 10)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));
        if (rng == null)
            throw new ArgumentNullException(nameof(rng));
        if (arr.Length == 0)
            return arr;

        for (int i = 0; i < arr.Length; i++)
            arr[i] = i;

        for (int i = 0; i < swaps; i++)
        {
            int a = rng.Next(0, arr.Length);
            int b = rng.Next(0, arr.Length);

            (arr[a], arr[b]) = (arr[b], arr[a]);
        }

        return arr;
    }


    public static int[] FillReversed(this int[] arr)
    {
        if (arr == null)
            throw new ArgumentNullException(nameof(arr));

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr.Length - i;
        }

        return arr;
    }
}