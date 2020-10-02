#include "Util.h"

void Util::Shuffle(int* arr, const int& count)
{
    int indexA, indexB, temp;
    srand((int)time(0));
    for (int i = 0; i < count * 10; i++) {
        indexA = rand() % count;
        indexB = rand() % count;
        temp = arr[indexA];
        arr[indexA] = arr[indexB];
        arr[indexB] = temp;
    }
}

int Util::GetCardRank(const int& n)
{
    if (n == 54) {
        return 14;
    }
    else if (n == 53) {
        return 13;
    }
    else if (n % 13 == 1) {
        return 12;
    }
    else if (n % 13 == 0) {
        return 11;
    }
    else {
        return n % 13 - 2;
    }
}

string Util::GetCardName(const int n)
{
    string str;
    if (n == 54) {

        str = "RJ";
    }
    else if (n == 53) {
        str = "LJ";
    }
    else if (n % 13 == 1) {
        str = "A";
    }
    else if (n % 13 == 0) {
        str = "K";
    }
    else if (n % 13 == 12) {
        str = "Q";
    }
    else if (n % 13 == 11) {
        str = "J";
    }
    else {
        str = to_string(n % 13);
    }
    return str;
}

int* Util::GetNums(int* arr, const int& count)
{
    int* res = new int[RANK_COUNT] {0};
    for (int i = 0; i < count; i++) {
        res[GetCardRank(arr[i])]++;
    }
    return res;
}

void Util::GetNumsAndTimes(int* arr, const int& count, int* arr_nums, int* arr_times, int& index_beg)
{
    index_beg = 0;
    arr_nums = GetNums(arr, count);
    arr_times = new int[TIMES_COUNT] {0};
    for (int i = 0; i < TIMES_COUNT; i++) {
        if (index_beg == 0 && arr_nums[i] != 0) {
            index_beg = i;
        }
        arr_times[arr_nums[i]]++;
    }
}

void Util::SortRank(int* arr, const int& count, bool littleFirst)
{
    int i, j, tmp, last, next;
    for (i = 0; i < count; i++) {
        for (j = count - 1; j > i; j--) {
            last = GetCardRank(arr[j - 1]);
            next = GetCardRank(arr[j]);
            if (littleFirst) {
                if (last < next) {
                    continue;
                }
            }
            else {
                if (last > next) {
                    continue;
                }
            }
            tmp = arr[j - 1];
            arr[j - 1] = arr[j];
            arr[j] = tmp;
        }
    }
}

void Util::SortNum(int* arr, const int& count, bool littleFirst)
{
    //每种扑克的张数
    int tmp_arr[RANK_COUNT]{ 0 };
    for (int i = 0; i < count; i++) {
        tmp_arr[GetCardRank(arr[i])]++;
    }
    //扑克的顺序
    int tmp_index[RANK_COUNT]{ 0 };
    for (int i = 0; i < RANK_COUNT; i++) {
        tmp_index[i] = i;
    }
    //用张数先给扑克排序，优先级
    int tmpa, tmpi;
    for (int i = 0; i < RANK_COUNT; i++) {
        for (int j = RANK_COUNT - 1; j > i; j--) {
            if (littleFirst) {
                if (tmp_arr[j - 1] < tmp_arr[j]) {
                    continue;
                }
            }
            else {
                if (tmp_arr[j - 1] > tmp_arr[j]) {
                    continue;
                }
            }
            tmpa = tmp_arr[j - 1];
            tmp_arr[j - 1] = tmp_arr[j];
            tmp_arr[j] = tmpa;

            tmpi = tmp_index[j - 1];
            tmp_index[j - 1] = tmp_index[j];
            tmp_index[j] = tmpi;

        }
    }
    //最后再用顺序对出牌进行排序
    int tmp, tmp_a, tmp_b;
    for (int i = 0; i < count; i++) {
        for (int j = count - 1; j > i; j--) {
            //找到两个元素对应的优先级,这地方应该可以用一个二维或者三维数组来记录
            for (int k = 0; k < RANK_COUNT; k++) {
                if (GetCardRank(arr[j - 1]) == tmp_index[k]) {
                    tmp_a = k;
                }
                if (GetCardRank(arr[j]) == tmp_index[k]) {
                    tmp_b = k;
                }
            }
            if (littleFirst) {
                if (tmp_a > tmp_b) {
                    continue;
                }
            }
            else {
                if (tmp_a < tmp_b) {
                    continue;
                }
            }
            tmp = arr[j - 1];
            arr[j - 1] = arr[j];
            arr[j] = tmp;
        }
    }
}

void Util::PrintSource(const int* arr, const int& count)
{
    cout << GetPrintSource(arr, count);
}

std::string Util::GetPrintSource(const int* arr, const int& count)
{
    string str;
    for (int i = 0; i < count; i++) {
        str += to_string(arr[i]) + " ";

    }
    str += "\n\n";
    return str;
}

void Util::PrintName(const int* arr, const int& count)
{
    cout << GetPrintName(arr, count);
}

std::string Util::GetPrintName(const int* arr, const int& count)
{
    string str;
    for (int i = 0; i < count; i++) {
        str += GetCardName(arr[i]);
        str += " ";
    }
    str += "\n\n";
    return str;
}

bool Util::IsSequence(const int* arr_num, const int beg, const int end)
{
    for (int i = beg; i < end; i++) {
        if (arr_num[i] == 0) {
            return false;
        }
    }
    return true;
}


