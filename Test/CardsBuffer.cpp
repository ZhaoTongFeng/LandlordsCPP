#include "CardsBuffer.h"
#include <iostream>

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
