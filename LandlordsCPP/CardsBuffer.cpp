#include "CardsBuffer.h"
#include <iostream>
#include "Util.h"

CardsBuffer::CardsBuffer(int size) :
    maxSize(size),
    top(-1)
{
    buf = new int[maxSize] {0};
}

CardsBuffer::CardsBuffer(CardsBuffer* target, bool move) :
    maxSize(target->GetMaxSize()), top(target->GetSize() - 1)
{
    int* arr = target->GetBuf();
    buf = new int[maxSize];
    int sz = sizeof(int) * GetSize();
    memcpy_s(buf, sz, arr, sz);
    if (move) {
        target->MakeEmpty();
    }
}

bool CardsBuffer::Push(const int& x)
{
    if (IsFull()) { return false; }
    buf[++top] = x;
    return true;
}

bool CardsBuffer::Get(int& x)
{
    if (IsEmpty()) { return false; }
    x = buf[top];
    return true;
}

bool CardsBuffer::Pop(int& x)
{
    if (IsEmpty()) { return false; }
    x = buf[top--];
    return true;
}

bool CardsBuffer::Pop(CardsBuffer* target)
{
    if (IsEmpty()) { return false; }
    int x = buf[top--];
    target->Push(x);
    return true;
}

bool CardsBuffer::Pop(CardsBuffer* target, int n)
{
    if (GetSize() < n || target->GetMaxSize() - target->GetSize() < n) { return false; }
    int x;
    for (int i = 0; i < n; i++) {
        x = buf[top--];
        target->Push(x);
    }
    return true;
}

void CardsBuffer::Shuffle()
{
    Util::Shuffle(buf, GetSize());
}

void CardsBuffer::SortRank(bool littleFirst)
{
    Util::SortRank(buf, GetSize(), littleFirst);
}

void CardsBuffer::SortNum(bool littleFirst)
{
    Util::SortNum(buf, GetSize(), littleFirst);
}



void CardsBuffer::GetNums(int* arr_nums)
{
    int size = GetSize();
    for (int i = 0; i < size; i++) {
        arr_nums[Util::GetCardRank(buf[i])]++;
    }
}

void CardsBuffer::GetNumsAndTimes(int* arr_nums, int* arr_times, int& index_beg)
{
    index_beg = 0;
    GetNums(arr_nums);

    for (int i = 0; i < RANK_COUNT; i++) {
        if (index_beg == 0 && arr_nums[i] != 0) {
            index_beg = i;
        }
        arr_times[arr_nums[i]]++;
    }
}



std::string CardsBuffer::GetPrintSource()
{
    return Util::GetPrintSource(buf, GetSize());
}


std::string CardsBuffer::GetPrintName()
{
    return Util::GetPrintName(buf, GetSize());
}
