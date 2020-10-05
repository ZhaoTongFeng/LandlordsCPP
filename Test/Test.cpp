// Test.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <time.h>
#include <random>
#include "SortCards.h"
#include "CardsBuffer.h"

using namespace std;

void Shuffle(int* arr,const int& count) {
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

void PrintSource(int* arr, const int& count) {
    for (int i = 0; i < count; i++) {
        cout << arr[i] << " ";
    }
    cout << endl << endl;
}

void PrintName(int* arr, const int& count) {
    for (int i = 0; i < count; i++) {
        if (arr[i] == 54) {
            cout << "RJ" << " ";
        }
        else if (arr[i] == 53) {
            cout << "LJ" << " ";
        }
        else if (arr[i] % 13 == 1) {
            cout << "A" << " ";
        }
        else if (arr[i] % 13 == 0) {
            cout << "K" << " ";
        }
        else if (arr[i] % 13 == 12) {
            cout << "Q" << " ";
        }
        else if (arr[i] % 13 == 11) {
            cout << "J" << " ";
        }
        else {
            cout << arr[i] % 13 << " ";
        }
    }
    cout << endl << endl;
}

int main()
{

    int arr[] = { 12,3,5,7,10,6,1,11,0 };

    int mx = 0, tmp = 0;
    for (int i = 1; i < sizeof(arr)/sizeof(int); i++) {
        if (arr[i] > arr[i-1]) {
            mx = max(tmp + abs(arr[i] - arr[i - 1]), mx);
        }
        else {
            tmp = 0;
        }
    }
    cout << mx;


    //sizeof只能测量int bbb[]这种类型的数组长度，无法测量int* arr这种动态分配的数组长度
    //int bbb[10];
    //cout << sizeof(bbb);

    //const int maxSize = 54;

    //CardsBuffer* all = new CardsBuffer(maxSize);

    //for (int i = 1; i <= maxSize; i++) {
    //    all->Push(i);
    //}

    //Shuffle(all->GetBuf(), all->GetSize());

    //CardsBuffer* copyCards = new CardsBuffer(all);

    //SortPointL(copyCards->GetBuf(), copyCards->GetSize());
    //PrintSource(copyCards->GetBuf(), copyCards->GetSize());
    //PrintName(copyCards->GetBuf(), copyCards->GetSize());
    
    //const int handMaxSize = 20;
    //CardsBuffer* handCards = new CardsBuffer(handMaxSize);

    //all->Pop(handCards, handMaxSize);

    //SortPointL(handCards->GetBuf(), handCards->GetSize());
    //PrintSource(handCards->GetBuf(), handCards->GetSize());
    //PrintName(handCards->GetBuf(), handCards->GetSize());





    //int* arr = new int[maxSize];
    //
    //for (int i = 0; i < maxSize; i++) {
    //    arr[i] = i + 1;
    //}


    //Shuffle(arr, maxSize);
    //PrintSource(arr, maxSize);
    //PrintName(arr, maxSize);

    //cout << "点数从小到大" << endl;
    //SortPointL(arr, maxSize);
    //SortPointB(arr, maxSize);
    //PrintSource(arr, maxSize);
    //PrintName(arr, maxSize);


    ////取出前17张牌模拟手牌
    //const int hand_count = 17;
    //int* hand_arr = new int[hand_count];
    //memcpy_s(hand_arr, hand_count * sizeof(int), arr, hand_count * sizeof(int));
    //PrintName(hand_arr, hand_count);

    ////cout << "张数从大到小" << endl;
    //SortNumB(hand_arr, hand_count);
    //PrintName(hand_arr, hand_count);
}