#pragma once

#ifdef MPIALGORITHMS_EXPORTS
#define MYLIBRARY_API __declspec(dllexport)
#else
#define MYLIBRARY_API __declspec(dllimport)
#endif

extern "C" {
	MYLIBRARY_API void quick_sort(int32_t* arr, int32_t len);
	MYLIBRARY_API void merge_sort(int32_t* arr, int32_t len);
	MYLIBRARY_API void heap_sort(int32_t* arr, int32_t len);
	MYLIBRARY_API void timsort(int32_t* arr, int32_t len);

	MYLIBRARY_API void bubble_sort(int32_t* arr, int32_t len);
	MYLIBRARY_API void insertion_sort(int32_t* arr, int32_t len);

	MYLIBRARY_API void bogo_sort(int32_t* arr, int32_t len);
	MYLIBRARY_API int32_t stalin_sort(int32_t* arr, int32_t len);
}