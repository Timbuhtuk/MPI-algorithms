#include "pch.h" ;
#include "sortings.h";
#include <cstdint>
#include <cstdlib>
#include <ctime>

static void swap_int(int32_t& a, int32_t& b)
{
    int32_t t = a;
    a = b;
    b = t;
}

/* =========================
   QUICK SORT
   ========================= */

static int partition_qs(int32_t* arr, int left, int right)
{
    int32_t pivot = arr[right];
    int i = left - 1;

    for (int j = left; j < right; j++)
    {
        if (arr[j] <= pivot)
        {
            i++;
            swap_int(arr[i], arr[j]);
        }
    }

    swap_int(arr[i + 1], arr[right]);
    return i + 1;
}

static void quick_sort_impl(int32_t* arr, int left, int right)
{
    if (left < right)
    {
        int pivotIndex = partition_qs(arr, left, right);
        quick_sort_impl(arr, left, pivotIndex - 1);
        quick_sort_impl(arr, pivotIndex + 1, right);
    }
}

void quick_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;
    quick_sort_impl(arr, 0, len - 1);
}

/* =========================
   MERGE SORT
   ========================= */

static void merge(int32_t* arr, int left, int mid, int right)
{
    int n1 = mid - left + 1;
    int n2 = right - mid;

    int32_t* L = new int32_t[n1];
    int32_t* R = new int32_t[n2];

    for (int i = 0; i < n1; i++)
        L[i] = arr[left + i];

    for (int j = 0; j < n2; j++)
        R[j] = arr[mid + 1 + j];

    int i = 0, j = 0, k = left;

    while (i < n1 && j < n2)
    {
        if (L[i] <= R[j])
            arr[k++] = L[i++];
        else
            arr[k++] = R[j++];
    }

    while (i < n1)
        arr[k++] = L[i++];

    while (j < n2)
        arr[k++] = R[j++];

    delete[] L;
    delete[] R;
}

static void merge_sort_impl(int32_t* arr, int left, int right)
{
    if (left < right)
    {
        int mid = left + (right - left) / 2;
        merge_sort_impl(arr, left, mid);
        merge_sort_impl(arr, mid + 1, right);
        merge(arr, left, mid, right);
    }
}

void merge_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;
    merge_sort_impl(arr, 0, len - 1);
}

/* =========================
   HEAP SORT
   ========================= */

static void heapify(int32_t* arr, int n, int i)
{
    int largest = i;
    int left = 2 * i + 1;
    int right = 2 * i + 2;

    if (left < n && arr[left] > arr[largest])
        largest = left;

    if (right < n && arr[right] > arr[largest])
        largest = right;

    if (largest != i)
    {
        swap_int(arr[i], arr[largest]);
        heapify(arr, n, largest);
    }
}

void heap_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;

    for (int i = len / 2 - 1; i >= 0; i--)
        heapify(arr, len, i);

    for (int i = len - 1; i > 0; i--)
    {
        swap_int(arr[0], arr[i]);
        heapify(arr, i, 0);
    }
}

/* =========================
   INSERTION SORT
   ========================= */

void insertion_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;

    for (int i = 1; i < len; i++)
    {
        int32_t key = arr[i];
        int j = i - 1;

        while (j >= 0 && arr[j] > key)
        {
            arr[j + 1] = arr[j];
            j--;
        }

        arr[j + 1] = key;
    }
}

/* =========================
   BUBBLE SORT
   ========================= */

void bubble_sort(int32_t* arr, int32_t len) {
	for (int q = 0; q < len; q++) {
		for (int e = 0; e < len - 1 - q; e++) {
			if (arr[e] > arr[e + 1]) {
				arr[e] += arr[e + 1];
				arr[e + 1] = arr[e] - arr[e + 1];
				arr[e] = arr[e] - arr[e + 1];
			}
		}
	}

	return;
}

/* =========================
   TIMSORT (simplified)
   ========================= */

static const int RUN = 32;

static void insertion_sort_range(int32_t* arr, int left, int right)
{
    for (int i = left + 1; i <= right; i++)
    {
        int32_t temp = arr[i];
        int j = i - 1;

        while (j >= left && arr[j] > temp)
        {
            arr[j + 1] = arr[j];
            j--;
        }

        arr[j + 1] = temp;
    }
}

static void merge_range(int32_t* arr, int left, int mid, int right)
{
    int len1 = mid - left + 1;
    int len2 = right - mid;

    int32_t* leftArr = new int32_t[len1];
    int32_t* rightArr = new int32_t[len2];

    for (int i = 0; i < len1; i++)
        leftArr[i] = arr[left + i];

    for (int i = 0; i < len2; i++)
        rightArr[i] = arr[mid + 1 + i];

    int i = 0, j = 0, k = left;

    while (i < len1 && j < len2)
    {
        if (leftArr[i] <= rightArr[j])
            arr[k++] = leftArr[i++];
        else
            arr[k++] = rightArr[j++];
    }

    while (i < len1)
        arr[k++] = leftArr[i++];

    while (j < len2)
        arr[k++] = rightArr[j++];

    delete[] leftArr;
    delete[] rightArr;
}

void timsort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;

    for (int i = 0; i < len; i += RUN)
    {
        int right = (i + RUN - 1 < len - 1) ? (i + RUN - 1) : (len - 1);
        insertion_sort_range(arr, i, right);
    }

    for (int size = RUN; size < len; size *= 2)
    {
        for (int left = 0; left < len; left += 2 * size)
        {
            int mid = left + size - 1;
            int right = left + 2 * size - 1;

            if (mid >= len)
                continue;

            if (right >= len)
                right = len - 1;

            merge_range(arr, left, mid, right);
        }
    }
}

/* =========================
   BOGO SORT
   ========================= */

static bool is_sorted(int32_t* arr, int32_t len)
{
    for (int i = 1; i < len; i++)
    {
        if (arr[i - 1] > arr[i])
            return false;
    }
    return true;
}

static void shuffle_array(int32_t* arr, int32_t len)
{
    for (int i = 0; i < len; i++)
    {
        int j = rand() % len;
        swap_int(arr[i], arr[j]);
    }
}

void bogo_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return;

    static bool seeded = false;
    if (!seeded)
    {
        srand((unsigned)time(nullptr));
        seeded = true;
    }

    while (!is_sorted(arr, len))
    {
        shuffle_array(arr, len);
    }
}

/* =========================
   STALIN SORT
   ========================= */

int32_t stalin_sort(int32_t* arr, int32_t len)
{
    if (!arr || len <= 1) return len;

    int32_t last_accepted = arr[0];
    int32_t* res = new int32_t[len];
    res[0] = last_accepted;

    int32_t e = 1;

    for (int q = 1; q < len; q++)
    {
        int32_t cur = arr[q];

        if (last_accepted <= cur)
        {
            res[e] = cur;
            last_accepted = cur;
            e++;
        }
    }

    for (int q = 0; q < e; q++)
    {
        arr[q] = res[q];
    }

    delete[] res;
    return e;
}

