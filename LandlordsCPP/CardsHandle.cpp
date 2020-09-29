#include "CardsHandle.h"
#include <time.h>
#include <random>


CardsHandle::CardsHandle(const int maxCount):
	BaseCardsHandle(maxCount)
{

}

CardsHandle::~CardsHandle()
{

}

void CardsHandle::Shuffle()
{
    //通过交换牌的位置打乱牌堆的顺序
    int indexA, indexB, temp;
    srand((int)time(0));
    for (int i = 0; i < MAX_CARDS_COUNT*10; i++) {
        indexA = rand() % MAX_CARDS_COUNT;
        indexB = rand() % MAX_CARDS_COUNT;
        temp = cards[indexA];
        cards[indexA] = cards[indexB];
        cards[indexB] = temp;
    }
    //恢复牌堆栈顶指针
    cards_ptr = MAX_CARDS_COUNT-1;
}


void CardsHandle::ComputeAttribute()
{
}
