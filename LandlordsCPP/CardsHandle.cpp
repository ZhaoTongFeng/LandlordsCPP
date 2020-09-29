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
    //ͨ�������Ƶ�λ�ô����ƶѵ�˳��
    int indexA, indexB, temp;
    srand((int)time(0));
    for (int i = 0; i < MAX_CARDS_COUNT*10; i++) {
        indexA = rand() % MAX_CARDS_COUNT;
        indexB = rand() % MAX_CARDS_COUNT;
        temp = cards[indexA];
        cards[indexA] = cards[indexB];
        cards[indexB] = temp;
    }
    //�ָ��ƶ�ջ��ָ��
    cards_ptr = MAX_CARDS_COUNT-1;
}


void CardsHandle::ComputeAttribute()
{
}
